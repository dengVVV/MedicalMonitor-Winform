using MedicalMonitor.WinForm.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceModel
    {
        /// <summary>
        /// 医疗设备的id大多数为这样：
        /// MONITOR_001
        /// M8000-10001
        /// PUMP-A01
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        public DeviceType DeviceType { get; set; }
        /// <summary>
        /// 串口
        /// </summary>
        public string PortName { get; set; } = string.Empty;
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }

        public bool IsOnline { get; set; }

        public DateTime ConnectedTime { get; set; }

        public DateTime LastDataReceivedTime { get; set; }
    }
}
