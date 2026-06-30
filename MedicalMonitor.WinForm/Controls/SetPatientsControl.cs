using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    public partial class SetPatientsControl : UIUserControl
    {
        private readonly MedicalMonitorDbContext _db;

        public event EventHandler SaveSuccess;

        public SetPatientsControl(MedicalMonitorDbContext db)
        {
            _db = db;
            InitializeComponent();
            AddFinish_btn.Click += AddFinish_btn_Click;
        }



        private void AddFinish_btn_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(PatinetAge_Value.Text, out int age))
            {
                UIMessageBox.ShowError("年龄格式错误");
                return;
            }

            DateTime admissionTime;

            if (!DateTime.TryParse(
                    AdmissionTime_Value.Text,
                    out admissionTime))
            {
                admissionTime = DateTime.Now;
            }

            PatientEntity entity = new PatientEntity
            {
                Name = PatientName_Value.Text.Trim(),
                BedNo = PatientBed_Value.Text.Trim(),
                Age = age,
                Gender = PatientGender_Value.Text.Trim(),
                Diagnosis = Diagnosis_Value.Text.Trim(),
                AdmissionTime = admissionTime,
                IsActive = true,
                Status = "住院中",
                BoundDeviceId = Guid.NewGuid().ToString("N"),
            };
            if (entity != null)
            {
                _db.Patients.Add(entity);
                _db.SaveChanges();
                UIMessageBox.ShowSuccess("添加成功！");
                SaveSuccess?.Invoke(this,EventArgs.Empty);
                //this.FindForm().Close();
                return;
            }
            UIMessageBox.ShowError("添加失败！请填入数据");
        }
    }
}
