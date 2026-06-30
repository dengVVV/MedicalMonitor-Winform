using MedicalMonitor.WinForm.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.Models
{
    /// <summary>
    /// 病人信息
    /// </summary>
    public class PatientModel
    {

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BedNo { get; set; } = string.Empty;
        public int Age { get; set; }

        public Gender Gender { get; set; }

        public string Diagnosis { get; set; } = string.Empty;
        public DateTime AdmissionTime { get; set; }

    }
}
