using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using MedicalMonitor.WinForm.Models;
using MedicalMonitor.WinForm.Models.Enums;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Timers;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 报警检测引擎 — 5 秒持续越界判断 + 报警防抖。
    ///
    /// 核心逻辑：
    ///   1. 每次收到生理参数快照，检查 6 个参数是否超出阈值
    ///   2. 越界 → 启动 5 秒计时窗口；恢复 → 取消计时
    ///   3. 5 秒后仍越界 → 触发报警事件（写入 DB + 通知 UI）
    ///   4. 同一参数持续越界期间只产生一条报警（防抖）
    ///   5. 多设备同时报警 → 各自独立触发
    ///
    /// 线程安全：ConcurrentDictionary + lock-free 设计。
    /// DB 写入和声音播放通过 Task.Run 异步执行，不阻塞检测线程。
    /// </summary>
    public class AlarmDetectionService : IDisposable
    {
        private readonly ThresholdConfigService _thresholdConfig;
        private readonly PatientBindingService _patientBinding;
        private readonly AlarmEventRepository _alarmRepo;

        private readonly ConcurrentDictionary<string, PendingAlarm> _pendingAlarms
            = new ConcurrentDictionary<string, PendingAlarm>();

        /// <summary>已触发报警集合（防抖）key="{DeviceId}_{VitalSignType}"</summary>
        private readonly ConcurrentDictionary<string, string> _activeAlarms
            = new ConcurrentDictionary<string, string>();

        private readonly Timer _pollTimer;
        private bool _disposed;

        public event EventHandler<AlarmEventModel> AlarmTriggered;
        public event EventHandler<string> AlarmResolved;

        public AlarmDetectionService(
            ThresholdConfigService thresholdConfig,
            PatientBindingService patientBinding,
            AlarmEventRepository alarmRepo)
        {
            _thresholdConfig = thresholdConfig;
            _patientBinding = patientBinding;
            _alarmRepo = alarmRepo;

            _pollTimer = new Timer(1000);
            _pollTimer.AutoReset = true;
            _pollTimer.Elapsed += OnPollTimerElapsed;
            _pollTimer.Start();
        }

        // ================================================================
        // 核心检测入口（在设备线程上调用）
        // ================================================================

        public void CheckVitalSigns(VitalSignsModel vitals)
        {
            if (vitals == null || !vitals.IsValid) return;

            var checks = new (VitalSignType type, double? value, string thresholdKey)[]
            {
                (VitalSignType.HeartRate,        vitals.HeartRate,        "HR"),
                (VitalSignType.SpO2,              vitals.SpO2,             "SpO2"),
                (VitalSignType.NIBP_Systolic,     vitals.NIBP_Systolic,    "NIBP_Systolic"),
                (VitalSignType.NIBP_Diastolic,    vitals.NIBP_Diastolic,   "NIBP_Diastolic"),
                (VitalSignType.RespiratoryRate,   vitals.RespiratoryRate,  "RESP"),
                (VitalSignType.Temperature,       vitals.Temperature,      "TEMP"),
            };

            foreach (var (paramType, value, thresholdKey) in checks)
            {
                if (value == null) continue;
                var threshold = _thresholdConfig.GetThreshold(thresholdKey);
                if (threshold == null) continue;

                bool isOutOfRange = value < threshold.LowLimit || value > threshold.HighLimit;
                string pendingKey = $"{vitals.DeviceId}_{(int)paramType}";

                if (isOutOfRange)
                {
                    if (!_activeAlarms.ContainsKey(pendingKey))
                    {
                        _pendingAlarms.AddOrUpdate(pendingKey,
                            _ => new PendingAlarm
                            {
                                DeviceId = vitals.DeviceId,
                                ParameterType = paramType,
                                FirstViolationTime = DateTime.Now,
                                ActualValue = value.Value,
                                ThresholdValue = value < threshold.LowLimit ? threshold.LowLimit : threshold.HighLimit,
                                IsLow = value < threshold.LowLimit,
                                AlarmLevel = DetermineAlarmLevel(paramType, value.Value, threshold),
                            },
                            (_, existing) => existing);
                    }
                }
                else
                {
                    if (_pendingAlarms.TryRemove(pendingKey, out _))
                    {
                        if (_activeAlarms.TryRemove(pendingKey, out var alarmId))
                        {
                            Log.Information("报警恢复: {Key}, AlarmId={AlarmId}", pendingKey, alarmId);
                            Task.Run(() => AlarmResolved?.Invoke(this, alarmId));
                        }
                    }
                }
            }
        }

        public void AcknowledgeAlarm(string alarmId, string operatorName)
        {
            try
            {
                _alarmRepo.Acknowledge(alarmId, operatorName);
                // 从活跃报警中移除
                foreach (var kv in _activeAlarms.Where(x => x.Value == alarmId).ToList())
                    _activeAlarms.TryRemove(kv.Key, out _);
                Log.Information("报警已确认: AlarmId={AlarmId}, By={Operator}", alarmId, operatorName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "确认报警失败 AlarmId={AlarmId}", alarmId);
            }
        }

        public List<AlarmEventEntity> GetUnacknowledgedAlarms()
        {
            try { return _alarmRepo.GetUnacknowledged(); }
            catch (Exception ex) { Log.Error(ex, "查询未确认报警失败"); return new List<AlarmEventEntity>(); }
        }

        public (List<AlarmEventEntity> Items, int Total) QueryPaged(
            int page, int pageSize, bool? acknowledged = null, int? level = null)
        {
            return _alarmRepo.QueryPaged(page, pageSize, acknowledged, level);
        }

        // ================================================================
        // 轮询定时器 — 检查到期的待定报警
        // ================================================================

        private void OnPollTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                var expiredKeys = new List<string>();

                foreach (var kv in _pendingAlarms)
                {
                    if ((now - kv.Value.FirstViolationTime).TotalSeconds >= 5)
                        expiredKeys.Add(kv.Key);
                }

                foreach (var key in expiredKeys)
                {
                    if (_pendingAlarms.TryRemove(key, out var pending))
                    {
                        if (_activeAlarms.ContainsKey(key)) continue;

                        var alarm = new AlarmEventModel
                        {
                            AlarmId = Guid.NewGuid().ToString("N").Substring(0, 32),
                            DeviceId = pending.DeviceId,
                            ParameterType = pending.ParameterType,
                            ActualValue = pending.ActualValue,
                            ThresholdValue = pending.ThresholdValue,
                            Level = pending.AlarmLevel,
                            TriggerTime = DateTime.Now,
                            PatientId = _patientBinding.GetPatientByDeviceId(pending.DeviceId)?.Id ?? string.Empty,
                            Message = FormatAlarmMessage(pending),
                        };

                        _activeAlarms.TryAdd(key, alarm.AlarmId);

                        var captured = alarm;
                        Task.Run(() =>
                        {
                            try
                            {
                                var entity = new AlarmEventEntity
                                {
                                    AlarmId = captured.AlarmId,
                                    PatientId = captured.PatientId,
                                    DeviceId = captured.DeviceId,
                                    ParameterType = VitSignTypeToDbByte(captured.ParameterType),
                                    ActualValue = captured.ActualValue,
                                    ThresholdValue = captured.ThresholdValue,
                                    AlarmLevel = AlarmLevelToDbByte(captured.Level),
                                    TriggerTime = captured.TriggerTime,
                                    Message = captured.Message,
                                };
                                _alarmRepo.Insert(entity);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "写入报警事件失败 AlarmId={AlarmId}", captured.AlarmId);
                            }
                        });

                        PlayAlarmSound(pending.AlarmLevel);
                        Task.Run(() => AlarmTriggered?.Invoke(this, alarm));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "报警轮询异常");
            }
        }

        // ================================================================
        // 辅助方法
        // ================================================================

        private static AlarmLevel DetermineAlarmLevel(
            VitalSignType paramType, double value, ThresholdConfigService.ParameterThreshold threshold)
        {
            double range = threshold.HighLimit - threshold.LowLimit;
            if (range <= 0) return AlarmLevel.Medium;

            double violationPercent = value < threshold.LowLimit
                ? (threshold.LowLimit - value) / range
                : (value - threshold.HighLimit) / range;

            if (violationPercent > 0.3) return AlarmLevel.High;
            if (violationPercent > 0.15) return AlarmLevel.Medium;
            return AlarmLevel.Low;
        }

        private static string FormatAlarmMessage(PendingAlarm pending)
        {
            string paramName;
            switch (pending.ParameterType)
            {
                case VitalSignType.HeartRate: paramName = "心率"; break;
                case VitalSignType.SpO2: paramName = "血氧饱和度"; break;
                case VitalSignType.NIBP_Systolic: paramName = "收缩压"; break;
                case VitalSignType.NIBP_Diastolic: paramName = "舒张压"; break;
                case VitalSignType.RespiratoryRate: paramName = "呼吸率"; break;
                case VitalSignType.Temperature: paramName = "体温"; break;
                default: paramName = pending.ParameterType.ToString(); break;
            }
            string direction = pending.IsLow ? "偏低" : "偏高";
            return $"{paramName}{direction}: 当前 {pending.ActualValue:F1}，阈值 {pending.ThresholdValue:F1}";
        }

        /// <summary>VitalSignType 枚举 → DB TINYINT (1=HR, 2=SpO2, 3=NIBP_SYS, ...)</summary>
        private static byte VitSignTypeToDbByte(VitalSignType type)
        {
            switch (type)
            {
                case VitalSignType.HeartRate: return 1;
                case VitalSignType.SpO2: return 2;
                case VitalSignType.NIBP_Systolic: return 3;
                case VitalSignType.NIBP_Diastolic: return 4;
                case VitalSignType.RespiratoryRate: return 5;
                case VitalSignType.Temperature: return 6;
                default: return 0;
            }
        }

        /// <summary>AlarmLevel 枚举 → DB TINYINT (1=High, 2=Medium, 3=Low)</summary>
        private static byte AlarmLevelToDbByte(AlarmLevel level)
        {
            switch (level)
            {
                case AlarmLevel.High: return 1;
                case AlarmLevel.Medium: return 2;
                case AlarmLevel.Low: return 3;
                default: return 2;
            }
        }

        private static void PlayAlarmSound(AlarmLevel level)
        {
            Task.Run(() =>
            {
                try
                {
                    string soundFile;
                    switch (level)
                    {
                        case AlarmLevel.High: soundFile = @"C:\Windows\Media\Windows Critical Stop.wav"; break;
                        case AlarmLevel.Medium: soundFile = @"C:\Windows\Media\Windows Exclamation.wav"; break;
                        default: soundFile = @"C:\Windows\Media\Windows Notify.wav"; break;
                    }
                    if (System.IO.File.Exists(soundFile))
                    {
                        using (var player = new SoundPlayer(soundFile))
                            player.Play();
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("播放报警声音失败: {Message}", ex.Message);
                }
            });
        }

        public void Dispose()
        {
            if (_disposed) return;
            _pollTimer?.Stop();
            _pollTimer?.Dispose();
            _disposed = true;
        }

        // ================================================================
        // 内部类型
        // ================================================================

        private class PendingAlarm
        {
            public string DeviceId { get; set; }
            public VitalSignType ParameterType { get; set; }
            public DateTime FirstViolationTime { get; set; }
            public double ActualValue { get; set; }
            public double ThresholdValue { get; set; }
            public bool IsLow { get; set; }
            public AlarmLevel AlarmLevel { get; set; }
        }
    }
}
