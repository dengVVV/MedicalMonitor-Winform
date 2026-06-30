using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 每日统计实体 — 映射 dbo.DailyStatistics 表
    /// 由 BackgroundStatisticsService 定时写入
    /// </summary>
    [Table("DailyStatistics")]
    public class DailyStatisticsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StatId { get; set; }

        [Required]
        [MaxLength(50)]
        [Index("IX_DailyStats_DeviceId_Date", Order = 0)]
        public string DeviceId { get; set; }

        [Index("IX_DailyStats_DeviceId_Date", Order = 1)]
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
        public DateTime CalculatedAt { get; set; }
    }
}
