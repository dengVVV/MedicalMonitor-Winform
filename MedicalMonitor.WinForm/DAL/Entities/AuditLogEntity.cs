using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 审计日志实体 — 映射 dbo.AuditLogs 表
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLogEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        public int UserId { get; set; }

        [MaxLength(50)]
        public string Action { get; set; }

        [MaxLength(50)]
        public string TableName { get; set; }

        [MaxLength(100)]
        public string RecordId { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
