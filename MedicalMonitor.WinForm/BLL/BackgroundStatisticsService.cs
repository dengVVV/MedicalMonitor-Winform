using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using Serilog;
using System;
using System.Linq;
using System.Timers;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 后台生理参数统计服务 — 每 20 分钟计算一次每日均值。
    ///
    /// 统计维度：
    ///   按设备分组 → AVG/MAX/MIN/STDEV → 写入 DailyStatistics 表
    ///
    /// 设计决策：
    ///   使用 System.Timers.Timer 在独立线程执行，不阻塞 UI。
    ///   首次执行在启动后延迟 2 分钟（给数据采集一个预热期），之后每 20 分钟一次。
    /// </summary>
    public class BackgroundStatisticsService : IDisposable
    {
        private readonly StatisticsRepository _statsRepo;

        /// <summary>统计执行定时器</summary>
        private readonly Timer _timer;

        /// <summary>统计间隔：20 分钟</summary>
        private const double StatisticsIntervalMs = 20 * 60 * 1000;

        /// <summary>首次执行延迟：2 分钟（数据预热）</summary>
        private const double InitialDelayMs = 2 * 60 * 1000;

        private bool _disposed;
        private bool _initialRunDone;

        public bool IsRunning => _timer?.Enabled ?? false;

        public BackgroundStatisticsService(StatisticsRepository statsRepo)
        {
            _statsRepo = statsRepo ?? throw new ArgumentNullException(nameof(statsRepo));

            _timer = new Timer(InitialDelayMs); // 首次用短间隔
            _timer.AutoReset = false;           // 单次触发，回调中切换为 20 分钟间隔
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BackgroundStatisticsService));
            _timer.Start();
            Log.Information("后台统计服务已启动，首次执行将在 2 分钟后");
        }

        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// 定时器回调：计算当日统计 → 写入数据库 → 安排下一次执行。
        /// 首次执行后切换为 20 分钟间隔。
        /// </summary>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ExecuteStatistics();
                Log.Information("每日统计计算完成（{Count} 个设备）",
                    _statsRepo.CalculateDailyStatistics(DateTime.Today).Count());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "每日统计计算失败");
            }
            finally
            {
                // 首次执行后切换为 20 分钟间隔
                if (!_initialRunDone)
                {
                    _initialRunDone = true;
                    _timer.Interval = StatisticsIntervalMs;
                    _timer.AutoReset = true;
                }
                _timer.Start(); // 安排下一次
            }
        }

        /// <summary>
        /// 执行统计计算并写入数据库。
        /// 计算流程：AVG/MAX/MIN/STDEV → MERGE INTO DailyStatistics
        /// </summary>
        private void ExecuteStatistics()
        {
            var stats = _statsRepo.CalculateDailyStatistics(DateTime.Today).ToList();
            if (stats.Count > 0)
            {
                _statsRepo.UpsertDailyStatistics(stats);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _timer?.Stop();
                _timer?.Dispose();
                _disposed = true;
            }
        }
    }
}
