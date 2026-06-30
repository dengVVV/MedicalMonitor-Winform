using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 报警事件实体 — 映射 dbo.AlarmEvents 表
    /// </summary>
    [Table("AlarmEvents")]
    public class AlarmEventEntity
    {
        [Key]
        [MaxLength(36)]
        public string AlarmId { get; set; }

        [MaxLength(50)]
        public string PatientId { get; set; }

        [MaxLength(50)]
        public string DeviceId { get; set; }

        /// <summary>生理参数类型: 1=HR 2=SpO2 3=NIBP_SYS 4=NIBP_DIA 5=RESP 6=TEMP</summary>
        public byte ParameterType { get; set; }

        public double ActualValue { get; set; }

        public double ThresholdValue { get; set; }

        /// <summary>报警等级: 1=高 2=中 3=低</summary>
        public byte AlarmLevel { get; set; }

        public DateTime TriggerTime { get; set; }

        public DateTime? AcknowledgedTime { get; set; }

        [MaxLength(50)]
        public string AcknowledgedBy { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
