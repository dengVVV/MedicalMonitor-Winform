using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 生理参数快照
    /// </summary>
    public class VitalSignsModel
    {
        /// <summary>
        /// 设备来源，通常为设备id
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 心率 bpm
        /// </summary>
        public double? HeartRate { get; set; }

        /// <summary>
        /// 血氧 %
        /// </summary>
        public double? SpO2 { get; set; }

        /// <summary>
        /// 呼吸频率 次/分钟
        /// </summary>
        public double? RespiratoryRate { get; set; }

        /// <summary>
        /// 体温 ℃
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// 收缩压 mmHg
        /// </summary>
        public double? NIBP_Systolic { get; set; }

        /// <summary>
        /// 舒张压 mmHg
        /// </summary>
        public double? NIBP_Diastolic { get; set; }

        /// <summary>
        /// 平均动脉压 mmHg
        /// </summary>
        public double? NibpMap { get; set; }
        /// <summary>
        /// 数据是否通过校验，是否有效
        /// </summary>
        public bool IsValid { get; set; }


        public override string ToString()
        {
            return $"HR:{HeartRate?.ToString() ?? "--"} " +
                   $"SpO2:{SpO2?.ToString() ?? "--"} " +
                   $"NIBP:{NIBP_Systolic?.ToString() ?? "--"}/{NIBP_Diastolic?.ToString() ?? "--"} " +
                   $"RESP:{RespiratoryRate?.ToString() ?? "--"} " +
                   $"TEMP:{Temperature?.ToString() ?? "--"} " +
                   $"| {Timestamp:HH:mm:ss}";
        }

    }
}
