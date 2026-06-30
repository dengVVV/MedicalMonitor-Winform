using MedicalMonitor.WinForm.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 报警事件
    /// </summary>
    public class AlarmEventModel
    {
        /// <summary>报警事件唯一标识（UUID）</summary>
        public string AlarmId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>触发报警的生理参数类型</summary>
        public VitalSignType ParameterType { get; set; }

        /// <summary>触发报警时的实际参数值</summary>
        public double ActualValue { get; set; }

        /// <summary>触发报警的阈值（高限或低限，取决于越界方向）</summary>
        public double ThresholdValue { get; set; }

        /// <summary>报警等级：高 / 中 / 低</summary>
        public AlarmLevel Level { get; set; }

        /// <summary>报警触发时间</summary>
        public DateTime TriggerTime { get; set; } = DateTime.Now;

        /// <summary>报警确认时间（null 表示尚未确认）</summary>
        public DateTime? AcknowledgedTime { get; set; }

        /// <summary>确认该报警的操作人（护士/医生工号）</summary>
        public string AcknowledgedBy { get; set; }

        /// <summary>患者ID，关联到对应患者</summary>
        public string PatientId { get; set; }

        /// <summary>设备ID，关联到触发报警的设备</summary>
        public string DeviceId { get; set; }

        /// <summary>报警是否已被确认（Acknowledge）</summary>
        public bool IsAcknowledged => AcknowledgedTime.HasValue;

        /// <summary>报警消息描述文本</summary>
        public string Message { get; set; }
    }
}
