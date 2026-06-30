using MedicalMonitor.WinForm.DAL;
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

namespace MedicalMonitor.WinForm.UI
{
    public partial class SetPatientsInfo : UIForm
    {
        private readonly MedicalMonitorDbContext _db;

        public SetPatientsInfo(MedicalMonitorDbContext db)
        {
            _db = db;
            InitializeComponent();

            setPatientsControl1.SaveSuccess += OnSaveSuccess;


        }

        private void OnSaveSuccess(object sender, EventArgs e)
        {
           this.DialogResult = DialogResult.OK;
           this.Close();
        }

    }
}
