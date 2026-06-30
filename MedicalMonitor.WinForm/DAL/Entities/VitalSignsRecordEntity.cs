using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 生理参数记录实体 — 映射 dbo.VitalSignsRecords 表
    /// 每 30 秒 BatchInsert 一批数据（Dapper），此处为 EF6 Code First 映射定义
    /// </summary>
    [Table("VitalSignsRecords")]
    public class VitalSignsRecordEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RecordId { get; set; }

        [Required]
        [MaxLength(50)]
        [Index("IX_VitalSigns_DeviceId", Order = 0)]
        public string DeviceId { get; set; }

        [MaxLength(20)]
        [Index("IX_VitalSigns_BedNo", Order = 0)]
        public string BedNo { get; set; }

        [Index("IX_VitalSigns_RecordTime", Order = 0)]
        public DateTime RecordTime { get; set; }

        public double? HeartRate { get; set; }
        public double? SpO2 { get; set; }
        public double? NibpSystolic { get; set; }
        public double? NibpDiastolic { get; set; }
        public double? RespiratoryRate { get; set; }
        public double? Temperature { get; set; }

        public bool IsValid { get; set; }
    }
}
