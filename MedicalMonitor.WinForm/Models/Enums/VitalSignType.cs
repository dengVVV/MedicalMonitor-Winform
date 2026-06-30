using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models.Enums
{
    /// <summary>
    /// 生理参数类型
    /// </summary>
    public enum VitalSignType
    {
        HeartRate = 0,
        /// <summary>
        /// 血氧饱和
        /// </summary>
        SpO2 = 1,
        /// <summary>
        /// 收缩压
        /// </summary>
        NIBP_Systolic = 2,
        /// <summary>
        /// 舒张压
        /// </summary>
        NIBP_Diastolic = 3,
        /// <summary>
        /// 呼吸率,次/分钟
        /// </summary>
        RespiratoryRate = 4,
        /// <summary>
        /// 体温
        /// </summary>
        Temperature = 5,

    }
}
