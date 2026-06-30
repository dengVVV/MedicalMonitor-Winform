using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models
{
    public class DeviceDataFrame
    {
        /// <summary>
        /// 原始数据ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 来源设备
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 原始字节数据
        /// </summary>
        public byte[] RawBytes { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataLength => RawBytes.Length;

        /// <summary>
        /// 十六进制字符串
        /// 调试协议时非常有用
        /// </summary>
        public string RawHex => BitConverter.ToString(RawBytes);

        /// <summary>
        /// 是否通过校验
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 解析时间
        /// </summary>
        public DateTime ParseTime { get; set; }
        /// <summary>
        /// 原始数据(字符串)
        /// </summary>
        public string RawData { get; set; }
        /// <summary>
        /// 收到时间
        /// </summary>
        public DateTime ReceivedTime { get; set; }
    }
}
