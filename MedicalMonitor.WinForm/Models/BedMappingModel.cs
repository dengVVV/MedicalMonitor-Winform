using System;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 床位-设备-患者 绑定关系模型。
    ///
    /// 业务含义：
    ///   在医院监护站中，每张床位（BED-01~BED-08）对应一台监护设备和一个患者。
    ///   该模型在三层架构中处于 Models 层，被 BLL.MonitoringService 和
    ///   BLL.PatientBindingService 共同引用。
    /// </summary>
    public class BedMappingModel
    {
        /// <summary>床位号，如 "BED-01" ~ "BED-08"</summary>
        public string BedNo { get; set; } = string.Empty;

        /// <summary>关联的设备ID，如 "SIM-001"</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>关联的患者信息（null 表示空床）</summary>
        public PatientModel Patient { get; set; }

        /// <summary>床位是否已被占用</summary>
        public bool IsOccupied => Patient != null && !string.IsNullOrEmpty(Patient.Name);

        /// <summary>设备是否在线（由 MonitoringService 维护）</summary>
        public bool IsOnline { get; set; }

        /// <summary>该床位最新一次生理参数快照（由 MonitoringService 更新）</summary>
        public VitalSignsModel LatestVitalSigns { get; set; }
    }
}
