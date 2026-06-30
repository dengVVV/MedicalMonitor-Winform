using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models.Enums
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 监护仪
        /// </summary>
        Monitor = 0,
        /// <summary>
        /// 输液泵
        /// </summary>
        InfusionPump = 1,
        /// <summary>
        /// 注射泵
        /// </summary>
        SyringePump = 2,


    }
}
