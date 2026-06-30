using MedicalMonitor.WinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.BLL.Interface
{
    public interface IDeviceDataSource
    {
        /// <summary>
        /// 当收到一帧新的胜利参数数据时触发
        /// 订阅者（解析者，ViewModel）通过此类事件获知新数据到达
        /// </summary>
        event EventHandler<VitalSignsModel> DataReceived;
        /// <summary>
        /// 当设备连接状态发生变化时触发（上线/离线/重连中）
        /// 订阅者（UI状态指示器）
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// 启动数据采集（打开串口=>开始模拟=>建立modbus连接）
        /// </summary>
        void Start();
        /// <summary>
        /// 停止数据采集（关闭串口=>停止模拟=>断开modbus连接）
        /// </summary>
        void Stop();
        /// <summary>
        /// 当前数据源是否处于运行状态
        /// </summary>
        bool IsRunning { get; }

    }
}
