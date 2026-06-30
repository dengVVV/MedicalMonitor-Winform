using MedicalMonitor.WinForm.Models;
using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    public partial class VitalSignPanelControl : UIUserControl
    {
        private VitalSignRow _rowHR, _rowSpO2, _rowNIBP, _rowRESP, _rowTEMP;

        public VitalSignPanelControl()
        {
            InitializeComponent();
        }

        public void UpdateVitalSigns(VitalSignsModel v)
        {
            if (v == null) return;
            _rowHR.SetValue(v.HeartRate);
            _rowSpO2.SetValue(v.SpO2);
            _rowNIBP.SetValue($"{v.NIBP_Systolic?.ToString("F0") ?? "--"}/{v.NIBP_Diastolic?.ToString("F0") ?? "--"}");
            _rowRESP.SetValue(v.RespiratoryRate);
            _rowTEMP.SetValue(v.Temperature);
        }

        private class VitalSignRow : UIPanel
        {
            private readonly string _name, _unit;
            private readonly Color _accent;
            private UILabel _lblName, _lblVal, _lblUnit;

            public VitalSignRow(string name, string unit, Color accent)
            {
                _name = name; _unit = unit; _accent = accent;
                this.SuspendLayout();
                this.Size = new Size(200, 65);
                this.FillColor = Color.Transparent;
                this.RectColor = Color.FromArgb(230, 235, 242);
                _lblName = new UILabel { Location = new Point(8, 2), Size = new Size(80, 18), Font = new Font("Microsoft YaHei", 8), ForeColor = Color.FromArgb(130, 130, 130), Text = _name };
                _lblVal = new UILabel { Location = new Point(8, 20), Size = new Size(130, 40), Font = new Font("Consolas", 22, FontStyle.Bold), ForeColor = _accent, Text = "--", TextAlign = ContentAlignment.MiddleLeft };
                _lblUnit = new UILabel { Location = new Point(140, 36), Size = new Size(50, 20), Font = new Font("Microsoft YaHei", 9), ForeColor = Color.FromArgb(150, 150, 150), Text = _unit };
                this.Controls.Add(_lblName); this.Controls.Add(_lblVal); this.Controls.Add(_lblUnit);
                this.ResumeLayout(false);
            }

            public void SetValue(double? v) { _lblVal.Text = v?.ToString("F0") ?? "--"; _lblVal.ForeColor = v.HasValue ? _accent : Color.Gray; }
            public void SetValue(string s) { _lblVal.Text = s ?? "--"; _lblVal.Font = new Font("Consolas", 18, FontStyle.Bold); }
        }
    }
}
