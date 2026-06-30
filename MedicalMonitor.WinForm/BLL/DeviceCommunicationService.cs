using MedicalMonitor.WinForm.Models;
using MedicalMonitor.WinForm.BLL.Interface;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 设备串口通信管理器。
    ///
    /// 业务背景：
    ///   医疗设备通过 RS232 串口与上位机工控机物理连接。
    ///   SerialPort 是 .NET Framework 原生支持的串口通信类，
    ///   无需额外第三方库即可完成串口打开、读取、写入操作。
    ///
    /// 核心职责：
    ///   1. 管理串口连接的生命周期（Open / Close / Reconnect）
    ///   2. 异步接收设备推送的文本数据帧
    ///   3. 心跳检测：如果超时未收到数据，判定设备离线并尝试自动重连
    ///   4. 将收到的原始数据通过 DataReceived 事件抛给解析器
    ///
    /// 重连策略：
    ///   断线后每隔 3 秒尝试一次重连，最多尝试 3 次。
    ///   3 次均失败后停止尝试，需用户手动干预（或系统管理员介入）。
    ///
    /// 注意：本类在开发阶段与 DeviceSimulatorService 共用 IDeviceDataSource 接口，
    /// 可以在 DI 容器中一键切换。
    /// </summary>
    public class DeviceCommunicationService : IDeviceDataSource, IDisposable
    {
        // ========== 事件 ==========
        public event EventHandler<VitalSignsModel> DataReceived;
        public event EventHandler<bool> ConnectionStatusChanged;

        // ========== 字段 ==========

        // 串口对象，.NET Framework 原生提供
        private SerialPort _serialPort;

        // 设备配置信息
        private readonly DeviceModel _device;

        // 重连定时器
        private readonly Timer _reconnectTimer;

        // 当前重连尝试次数
        private int _reconnectAttempts;

        // 最大重连次数（超过后放弃自动重连，等待人工介入）
        private const int MaxReconnectAttempts = 3;

        // 重连间隔（毫秒）
        private const double ReconnectIntervalMs = 3000;

        // 接收缓冲区（拼接多次 DataReceived 事件收到的分片数据）
        private string _receiveBuffer = string.Empty;

        // 是否已释放资源
        private bool _disposed;

        // ========== 属性 ==========
        public bool IsRunning => _serialPort != null && _serialPort.IsOpen;


        // ========== 构造函数 ==========

        /// <summary>
        /// 使用指定的设备信息初始化串口通信器。
        /// </summary>
        /// <param name="device">设备模型，携带串口名称、波特率等配置信息</param>
        public DeviceCommunicationService(DeviceModel device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _reconnectTimer = new Timer(ReconnectIntervalMs);
            _reconnectTimer.AutoReset = false;   // 单次触发，每次重连后手动决定是否继续
            _reconnectTimer.Elapsed += OnReconnectTimerElapsed;
        }

        /// <summary>
        /// 打开串口连接。
        /// 1. 配置串口参数（端口名、波特率、数据位、停止位、校验位）
        /// 2. 注册 DataReceived 事件回调
        /// 3. 打开串口
        /// 4. 通知外部连接状态变更
        ///
        /// 异常处理：
        ///   串口被占用或不存在时，向上层抛出异常，由调用方决定是否降级为模拟器。
        /// </summary>
        public void Start()
        {

            if (_disposed)
                throw new ObjectDisposedException(nameof(DeviceCommunicationService));
            try
            {
                _serialPort = new SerialPort
                {
                    PortName = _device.PortName,           // 如 "COM1", "COM3"
                    BaudRate = _device.BaudRate,           // 如 115200
                    DataBits = 8,                          // 大多数医疗设备使用 8 位数据位
                    StopBits = StopBits.One,               // 1 位停止位（标准配置）
                    Parity = Parity.None,                  // 无校验位（部分设备使用 Even/Odd）
                    ReadTimeout = 1000,                    // 读取超时 1 秒（避免永久阻塞）
                    NewLine = "\n"                         // 换行符作为帧结束标志
                };
                // SerialPort.DataReceived 事件在独立线程上触发，
                // 该线程是串口驱动程序的工作线程，不是 UI 线程。
                // 如果在事件回调中操作 UI 控件，需要 marshal 到 UI 线程。
                _serialPort.DataReceived += OnSerialDataReceived;
                _serialPort.Open();
                _device.IsOnline = true;
                _device.ConnectedTime = DateTime.Now;
                ConnectionStatusChanged?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                Log.Error($"{_device.PortName}串口{_device.PortName}打开失败：{ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// 关闭串口并停止重新连接尝试。
        /// </summary>
        public void Stop()
        {
            _reconnectTimer.Stop();
            _reconnectAttempts = 0;
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= OnSerialDataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            _device.IsOnline = false;
            ConnectionStatusChanged?.Invoke(this, false);
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _reconnectTimer.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// 串口数据接收事件回调。
        ///
        /// 注意事项：
        ///   1. 此方法在 SerialPort 的工作线程上执行，不是 UI 线程。
        ///   2. 串口传输是流式的——一次 DataReceived 事件可能收到不完整的一帧。
        ///      需要通过换行符 '\n' 判断帧边界。
        ///   3. 收到数据后更新最后一次数据接收时间（用于心跳检测）。
        /// </summary>
        private void OnSerialDataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            try
            {
                // 读取串口接收缓冲区中已有或新到达的数据
                string data = _serialPort.ReadExisting();

                if (string.IsNullOrEmpty(data))
                    return;

                // 追加到接收缓冲区，再按行分割出完整帧
                _receiveBuffer += data;

                // 逐帧处理：以 '\n' 为帧结束标识
                // 例如设备可能一次发送: "HR:75,SpO2:98,NIBP:120/80,RESP:16,TEMP:36.8\n"
                while (_receiveBuffer.Contains("\n"))
                {
                    //在字符串中查找某个字符或子字符串第一次出现的位置
                    int newlineIndex = _receiveBuffer.IndexOf('\n');
                    string frame = _receiveBuffer.Substring(0, newlineIndex).Trim();

                    // 移除已处理的部分（包括换行符本身）
                    _receiveBuffer = _receiveBuffer.Substring(newlineIndex + 1);

                    // 跳过空帧（纯换行符）
                    if (string.IsNullOrWhiteSpace(frame))
                        continue;

                    // 更新设备最后接收时间（心跳检测用）
                    _device.LastDataReceivedTime = DateTime.Now;

                    // 构建原始数据帧（供审计日志使用）
                    var dataFrame = new DeviceDataFrame
                    {
                        RawData = frame,
                        ReceivedTime = DateTime.Now,
                        DeviceId = _device.DeviceId,
                        IsValid = true   // 原始帧此时尚未校验，解析器负责校验
                    };

                    // 将原始帧文本和帧对象分别传递给解析器
                    // 注意：此阶段不做解析，只是将原始数据通过事件上抛
                    // 解析器订阅此事件后负责将帧转为 VitalSignsModel
                    OnFrameReceived(frame, dataFrame);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeviceComm] 数据接收异常: {ex.Message}");
            }
        }



        /// <summary>
        /// 帧处理：将收到的原始帧通过事件通知给订阅者（解析器）。
        ///
        /// 设计说明：
        ///   此处不做解析的原因是遵循"单一职责原则"。
        ///   通信层只负责"拿到数据"，解析层负责"理解数据"。
        ///   分拆后每个类的测试复杂度大幅降低。
        /// </summary>
        private void OnFrameReceived(string rawFrame, DeviceDataFrame dataFrame)
        {
            // 此阶段没有 VitalSignsModel——解析器会在 DataParserService 中完成转换。
            // 通信层的 DataReceived 事件传递一个"占位"对象，包含原始帧信息。
            // 真实项目中，这里可以通过一个中间事件（DataFrameReceived）先通知解析器，
            // 解析器转换后再触发 DataReceived(VitalSignsModel)。

            // 简化处理：本阶段直接通过 Console 输出，具体连接在 Program.cs 中完成。
            Console.WriteLine($"[DeviceComm] 收到原始帧: {rawFrame}");
        }


        /// <summary>
        /// 重连定时器回调。
        /// 每隔 ReconnectIntervalMs 毫秒尝试一次重连，
        /// 直到成功（停止定时器）或超过最大重试次数（放弃）。
        /// </summary>
        private void OnReconnectTimerElapsed(object sender,ElapsedEventArgs e)
        {
            try
            {
                _reconnectAttempts++;
                Console.WriteLine($"尝试重连({_reconnectAttempts}/{MaxReconnectAttempts})");
                if (_serialPort == null)
                {
                    _serialPort = new SerialPort(_device.PortName, _device.BaudRate);
                    _serialPort.DataReceived += OnSerialDataReceived;
                }
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
                //重连成功
                _device.IsOnline = true;
                _reconnectAttempts = 0;
                ConnectionStatusChanged?.Invoke(this, true);
                Console.WriteLine($"重连成功：{_device.PortName}");
            }
            catch (Exception ex)
            {
                //重连失败
                if (_reconnectAttempts >= MaxReconnectAttempts)
                {
                    Console.WriteLine($"重连失败，已达到最大重试次数:{MaxReconnectAttempts}");
                    _reconnectTimer.Stop();
                    _device.IsOnline = false;
                    ConnectionStatusChanged?.Invoke(this,false);
                }
                else
                {
                    //继续尝试
                    _reconnectTimer.Start();
                }
            }
        }






    }
}
