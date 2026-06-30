using MedicalMonitor.WinForm.BLL.Interface;
using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using MedicalMonitor.WinForm.Models;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 多设备监控数据编排器 — BLL 层核心中枢。
    ///
    /// 职责边界：
    ///   1. 管理多个 IDeviceDataSource 实例（模拟器/串口），支持运行时动态添加
    ///   2. 接收各设备 DataReceived 事件，解析并维护最新快照
    ///   3. 每 30 秒批量写入生理参数到 SQL Server
    ///   4. 通过 SnapshotUpdated 事件通知 UI 层刷新
    ///
    /// 线程安全：
    ///   ConcurrentDictionary 保证多设备并发写入线程安全。
    ///   Timer 在独立线程触发，UI 通过 Invoke 方式 marshal 回 UI 线程。
    /// </summary>
    public class MonitoringService : IDisposable
    {
        // ========== 依赖注入 ==========
        private readonly DataParserService _parser;
        private readonly VitalSignsQueryRepository _vitalSignsRepo;
        private readonly PatientBindingService _patientBinding;
        private readonly AlarmDetectionService _alarmService;

        // ========== 动态数据源管理 ==========

        /// <summary>所有已注册的设备数据源（线程安全）</summary>
        private readonly ConcurrentDictionary<string, IDeviceDataSource> _dataSources
            = new ConcurrentDictionary<string, IDeviceDataSource>();

        // ========== 数据存储 ==========

        /// <summary>每个设备的最新生理参数快照（线程安全）</summary>
        private readonly ConcurrentDictionary<string, VitalSignsModel> _snapshots
            = new ConcurrentDictionary<string, VitalSignsModel>();

        /// <summary>待批量入库的缓冲队列（线程安全）</summary>
        private readonly ConcurrentQueue<VitalSignsRecordEntity> _pendingRecords
            = new ConcurrentQueue<VitalSignsRecordEntity>();

        // ========== 定时器 ==========

        /// <summary>批量入库定时器：每 30 秒执行一次</summary>
        private readonly Timer _batchInsertTimer;

        private const double BatchInsertIntervalMs = 30000;

        // ========== 事件 ==========

        /// <summary>当任一设备产生新数据时触发。UI 层订阅此事件刷新显示。</summary>
        public event EventHandler<VitalSignsModel> SnapshotUpdated;

        /// <summary>当设备连接状态变更时触发</summary>
        public event EventHandler<(string DeviceId, bool IsOnline)> ConnectionChanged;

        /// <summary>是否正在运行</summary>
        public bool IsRunning { get; private set; }

        /// <summary>当前所有快照的只读副本（供 UI 初始化或全量刷新）</summary>
        public IReadOnlyDictionary<string, VitalSignsModel> AllSnapshots => _snapshots;

        /// <summary>获取指定设备的最新快照</summary>
        public VitalSignsModel GetSnapshot(string deviceId)
        {
            _snapshots.TryGetValue(deviceId, out var snapshot);
            return snapshot;
        }

        /// <summary>当前已注册的设备数量</summary>
        public int DataSourceCount => _dataSources.Count;

        // ========== 构造函数 ==========

        public MonitoringService(
            DataParserService parser,
            VitalSignsQueryRepository vitalSignsRepo,
            PatientBindingService patientBinding,
            AlarmDetectionService alarmService)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _vitalSignsRepo = vitalSignsRepo ?? throw new ArgumentNullException(nameof(vitalSignsRepo));
            _patientBinding = patientBinding;
            _alarmService = alarmService ?? throw new ArgumentNullException(nameof(alarmService));

            _batchInsertTimer = new Timer(BatchInsertIntervalMs);
            _batchInsertTimer.AutoReset = true;
            _batchInsertTimer.Elapsed += OnBatchInsertTimerElapsed;
        }

        // ========== 动态数据源管理 ==========

        /// <summary>
        /// 运行时添加设备数据源。若同 DeviceId 已存在则先移除旧的再添加新的。
        /// 如果服务已在运行，新数据源会立即启动。
        /// </summary>
        public void AddDataSource(IDeviceDataSource source)
        {
            if (source == null) return;
            string deviceId = (source is DeviceSimulatorService sim) ? sim.DeviceId : source.GetHashCode()
                .ToString("X8");
            Log.Information("添加设备数据源: {DeviceId}", deviceId);

            _dataSources.AddOrUpdate(deviceId, source, (key, old) =>
            {
                old.DataReceived -= OnDeviceDataReceived;
                old.ConnectionStatusChanged -= OnConnectionStatusChanged;
                if (IsRunning) old.Stop();
                return source;
            });

            source.DataReceived += OnDeviceDataReceived;
            source.ConnectionStatusChanged += OnConnectionStatusChanged;

            if (IsRunning)
                source.Start();
        }

        // ========== 生命周期管理 ==========

        /// <summary>
        /// 启动所有已注册的设备数据源和批量入库定时器。
        /// </summary>
        public void StartAll()
        {
            if (IsRunning) return;

            foreach (var kv in _dataSources)
                kv.Value.Start();

            _batchInsertTimer.Start();
            IsRunning = true;
            Log.Information("MonitoringService 已启动，管理 {Count} 个设备", _dataSources.Count);
        }

        /// <summary>
        /// 停止所有设备数据源和定时器。
        /// </summary>
        public void StopAll()
        {
            if (!IsRunning) return;

            _batchInsertTimer.Stop();
            FlushPendingRecords();

            foreach (var kv in _dataSources)
                kv.Value.Stop();

            IsRunning = false;
            Log.Information("MonitoringService 已停止");
        }

        // ========== 数据处理 ==========

        private void OnDeviceDataReceived(object sender, VitalSignsModel vitals)
        {
            if (vitals == null) return;

            try
            {
                _snapshots[vitals.DeviceId] = vitals;

                var record = new VitalSignsRecordEntity
                {
                    DeviceId = vitals.DeviceId,
                    BedNo = _patientBinding.GetBedNoByDeviceId(vitals.DeviceId) ?? string.Empty,
                    RecordTime = vitals.Timestamp,
                    HeartRate = vitals.HeartRate,
                    SpO2 = vitals.SpO2,
                    NibpSystolic = vitals.NIBP_Systolic,
                    NibpDiastolic = vitals.NIBP_Diastolic,
                    RespiratoryRate = vitals.RespiratoryRate,
                    Temperature = vitals.Temperature,
                    IsValid = vitals.IsValid
                };
                _pendingRecords.Enqueue(record);

                SnapshotUpdated?.Invoke(this, vitals);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "处理设备数据时异常 DeviceId={DeviceId}", vitals.DeviceId);
            }
        }

        private void OnConnectionStatusChanged(object sender, bool isOnline)
        {
            if (sender is DeviceSimulatorService sim)
            {
                ConnectionChanged?.Invoke(this, (sim.DeviceId, isOnline));
            }
        }

        // ========== 批量入库 ==========

        private void OnBatchInsertTimerElapsed(object sender, ElapsedEventArgs e)
        {
            FlushPendingRecords();
        }

        private void FlushPendingRecords()
        {
            if (_pendingRecords.IsEmpty) return;

            try
            {
                if (_pendingRecords.Count > 500)
                {
                    Log.Warning("待入库队列过长 ({Count})，触发过载保护", _pendingRecords.Count);
                    for (int i = 0; i < _pendingRecords.Count / 2; i++)
                        _pendingRecords.TryDequeue(out _);
                }

                var batch = new List<VitalSignsRecordEntity>();
                while (batch.Count < 200 && _pendingRecords.TryDequeue(out var record))
                {
                    batch.Add(record);
                }

                if (batch.Count > 0)
                {
                    _vitalSignsRepo.BatchInsert(batch);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "批量写入生理参数失败");
            }
        }

        // ========== 释放资源 ==========

        public void Dispose()
        {
            StopAll();
            _batchInsertTimer?.Dispose();
        }
    }
}
