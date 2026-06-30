using MedicalMonitor.WinForm.Models;
using MedicalMonitor.WinForm.BLL.Interface;
using Serilog;
using System;
using System.Timers;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 设备数据模拟器 — 每个实例模拟一个独立患者的生理参数。
    ///
    /// 改进点：
    ///   1. 使用 Guid + DeviceId 哈希确保每个实例有唯一种子
    ///   2. 支持患者基线参数，不同患者围绕不同基线波动
    ///   3. 内置异常注入定时器，为报警引擎测试提供真实数据
    ///
    /// 实现 IDeviceDataSource 接口，使得上层代码可以无缝切换到真实通信器。
    /// </summary>
    public class DeviceSimulatorService : IDeviceDataSource, IDisposable
    {
        public string DeviceId { get; }

        public event EventHandler<VitalSignsModel> DataReceived;

        public event EventHandler<bool> ConnectionStatusChanged;

        private readonly Timer _timer;

        /// <summary>随机数生成器 — 使用 Guid 哈希确保每个实例有不同的种子</summary>
        private readonly Random _random;

        /// <summary>患者基线生理参数 — 每个实例围绕不同的基线波动</summary>
        private readonly double _baselineHR, _baselineSpO2, _baselineSYS, _baselineDIA, _baselineRESP, _baselineTEMP;

        /// <summary>异常注入定时器（随机让参数越界用于测试报警引擎）</summary>
        private readonly Timer _anomalyTimer;
        private bool _inAnomaly;

        /// <summary>数据刷新间隔，2s一次，和真实监护仪对标</summary>
        private const double RefreshIntervalMs = 2000;

        private bool _disposed;

        public bool IsRunning => _timer?.Enabled ?? false;

        /// <summary>
        /// 创建模拟器实例。
        /// </summary>
        /// <param name="deviceId">设备唯一标识，如 "SIM-001"</param>
        /// <param name="baselineHR">心率基线 (bpm)</param>
        /// <param name="baselineSpO2">血氧基线 (%)</param>
        /// <param name="baselineSYS">收缩压基线 (mmHg)</param>
        /// <param name="baselineDIA">舒张压基线 (mmHg)</param>
        /// <param name="baselineRESP">呼吸率基线 (次/分)</param>
        /// <param name="baselineTEMP">体温基线 (℃)</param>
        public DeviceSimulatorService(
            string deviceId,
            double baselineHR = 80,
            double baselineSpO2 = 97,
            double baselineSYS = 130,
            double baselineDIA = 80,
            double baselineRESP = 16,
            double baselineTEMP = 36.8)
        {
            DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
            // 使用 Guid 哈希 + deviceId 哈希确保每个模拟器有唯一随机种子
            _random = new Random(Guid.NewGuid().GetHashCode() ^ deviceId.GetHashCode());

            _baselineHR = baselineHR;
            _baselineSpO2 = baselineSpO2;
            _baselineSYS = baselineSYS;
            _baselineDIA = baselineDIA;
            _baselineRESP = baselineRESP;
            _baselineTEMP = baselineTEMP;

            _timer = new Timer(RefreshIntervalMs);
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimerElapsed;

            // 异常注入定时器：每 30~90 秒随机让一个参数短暂越界
            _anomalyTimer = new Timer(30000 + _random.NextDouble() * 60000);
            _anomalyTimer.AutoReset = true;
            _anomalyTimer.Elapsed += OnAnomalyTimerElapsed;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DeviceSimulatorService));
            ConnectionStatusChanged?.Invoke(this, true);
            _timer.Start();
            _anomalyTimer.Start();
        }

        public void Stop()
        {
            if (_disposed)
                return;
            _timer.Stop();
            _anomalyTimer.Stop();
            ConnectionStatusChanged?.Invoke(this, false);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _timer?.Stop();
                _timer?.Dispose();
                _anomalyTimer?.Stop();
                _anomalyTimer?.Dispose();
                _disposed = true;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var vitals = GenerateVitalSigns();
                DataReceived?.Invoke(this, vitals);
            }
            catch (Exception ex)
            {
                Log.Error($"设备模拟器生成数据时发生异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 生成一组围绕患者基线波动的模拟生理参数。
        /// 每个值 = 患者基线 ± 随机波动
        /// </summary>
        private VitalSignsModel GenerateVitalSigns()
        {
            double hr, spo2, sys, dia, resp, temp;

            if (_inAnomaly)
            {
                // 异常模式：选择一个参数大幅偏离
                int anomalyParam = _random.Next(0, 5);
                hr   = anomalyParam == 0 ? _baselineHR  + (_random.NextDouble() > 0.5 ? 1 : -1) * (30 + _random.NextDouble() * 20) : BaselineRand(_baselineHR,   15);
                spo2 = anomalyParam == 1 ? Math.Max(70, _baselineSpO2 - 8 - _random.NextDouble() * 10) : BaselineRand(_baselineSpO2, 3);
                sys  = anomalyParam == 2 ? _baselineSYS  + (_random.NextDouble() > 0.5 ? 1 : -1) * (30 + _random.NextDouble() * 20) : BaselineRand(_baselineSYS, 15);
                dia  = anomalyParam == 3 ? _baselineDIA  + (_random.NextDouble() > 0.5 ? 1 : -1) * (20 + _random.NextDouble() * 10) : BaselineRand(_baselineDIA, 10);
                resp = anomalyParam == 4 ? _baselineRESP + (_random.NextDouble() > 0.5 ? 1 : -1) * (10 + _random.NextDouble() * 8) : BaselineRand(_baselineRESP, 4);
                temp = BaselineRand(_baselineTEMP, 0.5);
            }
            else
            {
                hr   = BaselineRand(_baselineHR,   15);
                spo2 = BaselineRand(_baselineSpO2,  3);
                sys  = BaselineRand(_baselineSYS,   15);
                dia  = BaselineRand(_baselineDIA,   10);
                resp = BaselineRand(_baselineRESP,   4);
                temp = BaselineRand(_baselineTEMP, 0.5);
            }

            var vitals = new VitalSignsModel
            {
                Timestamp = DateTime.Now,
                DeviceId = DeviceId,
                IsValid = true,
                HeartRate        = Clamp(Math.Round(hr,   0), 30, 220),
                SpO2             = Clamp(Math.Round(spo2, 0), 50, 100),
                NIBP_Systolic    = Clamp(Math.Round(sys,  0), 50, 260),
                NIBP_Diastolic   = Clamp(Math.Round(dia,  0), 20, 160),
                RespiratoryRate  = Clamp(Math.Round(resp, 0),  0,  60),
                Temperature      = Clamp(Math.Round(temp, 1), 33.0, 42.0)
            };

            return vitals;
        }

        /// <summary>基线 ± 随机波动</summary>
        private double BaselineRand(double baseline, double amplitude)
        {
            return baseline + (_random.NextDouble() - 0.5) * 2 * amplitude;
        }

        /// <summary>钳位到生理参数有效范围</summary>
        private static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>异常注入定时器：随机切换异常/正常模式</summary>
        private void OnAnomalyTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _inAnomaly = !_inAnomaly;
            // 异常持续 5~8 秒（覆盖报警引擎 5 秒检测窗口）
            if (_inAnomaly)
                _anomalyTimer.Interval = 5000 + _random.NextDouble() * 3000;
            else
                _anomalyTimer.Interval = 30000 + _random.NextDouble() * 60000;
        }
    }
}
