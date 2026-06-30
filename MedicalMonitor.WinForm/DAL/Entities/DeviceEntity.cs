using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 设备实体 — 映射 dbo.Devices 表
    /// </summary>
    [Table("Devices")]
    public class DeviceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index("IX_Devices_DeviceId", IsUnique = true)]
        public string DeviceId { get; set; }

        [MaxLength(30)]
        public string DeviceType { get; set; }

        [MaxLength(20)]
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? ConnectedTime { get; set; }

        public DateTime? LastDataReceivedTime { get; set; }
    }
}
