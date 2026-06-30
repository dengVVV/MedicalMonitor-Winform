using System;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 每日统计结果模型。
    /// 由 BackgroundStatisticsService 计算后填充，供 UI 统计摘要栏展示。
    /// </summary>
    public class DailyStatisticsModel
    {
        public string DeviceId { get; set; }
        public DateTime StatDate { get; set; }

        public double? AvgHeartRate { get; set; }
        public double? AvgSpO2 { get; set; }
        public double? AvgNibpSystolic { get; set; }
        public double? AvgNibpDiastolic { get; set; }
        public double? AvgRespiratoryRate { get; set; }
        public double? AvgTemperature { get; set; }

        public double? MaxHeartRate { get; set; }
        public double? MinHeartRate { get; set; }
        public double? StdDevHeartRate { get; set; }

        public int RecordCount { get; set; }

        /// <summary>全部设备的平均心率（DashboardForm 底部统计栏用）</summary>
        public static DailyStatisticsModel CreateOverall(
            double? avgHr, double? avgSpO2, int totalRecords, int onlineDevices)
        {
            return new DailyStatisticsModel
            {
                AvgHeartRate = avgHr,
                AvgSpO2 = avgSpO2,
                RecordCount = totalRecords,
                StatDate = DateTime.Today
            };
        }
    }
}
