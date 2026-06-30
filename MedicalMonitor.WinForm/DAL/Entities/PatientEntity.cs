using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 患者实体 — 映射 dbo.Patients 表
    /// </summary>
    [Table("Patients")]
    public class PatientEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PatientId { get; set; }

        [Required]
        [MaxLength(50)]
        [Index("IX_Patients_Name", Order = 0)]
        public string Name { get; set; }

        [MaxLength(20)]
        [Index("IX_Patients_BedNo", Order = 0)]
        [Column("BedNumber")]
        public string BedNo { get; set; }

        public int Age { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(500)]
        public string Diagnosis { get; set; }

        public DateTime AdmissionTime { get; set; }

        public string BoundDeviceId { get; set; }

        public string Status { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
    }
}
