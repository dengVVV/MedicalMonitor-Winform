using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    partial class AlarmPopupForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._lblTitle = new UILabel();
            this._lblPatient = new UILabel();
            this._lblParameter = new UILabel();
            this._lblTime = new UILabel();
            this._btnAcknowledge = new UIButton();

            this.SuspendLayout();

            // AlarmPopupForm
            this.ClientSize = new Size(380, 220);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "⚠ 设备报警";
            this.ShowTitleIcon = true;
            this.TopMost = true;
            this.MinimumSize = new Size(350, 200);

            // _lblTitle
            this._lblTitle.Location = new Point(15, 12);
            this._lblTitle.Size = new Size(350, 28);
            this._lblTitle.Font = new Font("Microsoft YaHei", 14, FontStyle.Bold);
            this._lblTitle.ForeColor = Color.FromArgb(180, 30, 30);
            this._lblTitle.BackColor = Color.Transparent;
            this._lblTitle.AutoSize = false;

            // _lblPatient
            this._lblPatient.Location = new Point(15, 50);
            this._lblPatient.Size = new Size(350, 24);
            this._lblPatient.Font = new Font("Microsoft YaHei", 11, FontStyle.Bold);
            this._lblPatient.ForeColor = Color.FromArgb(50, 50, 50);
            this._lblPatient.BackColor = Color.Transparent;
            this._lblPatient.AutoSize = false;

            // _lblParameter
            this._lblParameter.Location = new Point(15, 82);
            this._lblParameter.Size = new Size(350, 48);
            this._lblParameter.Font = new Font("Microsoft YaHei", 11);
            this._lblParameter.ForeColor = Color.FromArgb(80, 80, 80);
            this._lblParameter.BackColor = Color.Transparent;
            this._lblParameter.AutoSize = false;

            // _lblTime
            this._lblTime.Location = new Point(15, 135);
            this._lblTime.Size = new Size(200, 24);
            this._lblTime.Font = new Font("Microsoft YaHei", 9);
            this._lblTime.ForeColor = Color.FromArgb(120, 120, 120);
            this._lblTime.BackColor = Color.Transparent;
            this._lblTime.AutoSize = false;

            // _btnAcknowledge
            this._btnAcknowledge.Location = new Point(230, 160);
            this._btnAcknowledge.Size = new Size(120, 36);
            this._btnAcknowledge.Text = "确认报警";
            this._btnAcknowledge.Style = UIStyle.Blue;
            this._btnAcknowledge.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);

            // Controls
            this.Controls.AddRange(new Control[] {
                _lblTitle, _lblPatient, _lblParameter, _lblTime, _btnAcknowledge
            });

            this.ResumeLayout(false);
        }
    }
}
