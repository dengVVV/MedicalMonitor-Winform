using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    partial class BedCardControl
    {
        private void InitializeComponent()
        {
            this._lblBedNo = new UILabel();
            this._lblPatientName = new UILabel();
            this._lblPatientAge = new UILabel();
            this._lblHR = new UILabel();
            this._lblSpO2 = new UILabel();
            this._lblNIBP = new UILabel();
            this._statusLight = new UILight();
            this._lblStatus = new UILabel();
            UILine sep = new UILine();   // var 改成显式类型

            this.SuspendLayout();

            // lblBedNo
            this._lblBedNo.Location = new System.Drawing.Point(12, 10);
            this._lblBedNo.Size = new System.Drawing.Size(100, 28);
            this._lblBedNo.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Bold);
            this._lblBedNo.ForeColor = System.Drawing.Color.FromArgb(45, 120, 210);
            this._lblBedNo.Text = "BED-01";

            // lblPatientName
            this._lblPatientName.Location = new System.Drawing.Point(120, 12);
            this._lblPatientName.Size = new System.Drawing.Size(130, 24);
            this._lblPatientName.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this._lblPatientName.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._lblPatientName.Text = "(空床)";
            this._lblPatientName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;


            // lblPatientAge
            this._lblPatientAge.Location = new System.Drawing.Point(120, 34);
            this._lblPatientAge.Size = new System.Drawing.Size(130, 20);
            this._lblPatientAge.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this._lblPatientAge.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this._lblPatientAge.Text = "";
            this._lblPatientAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // lblHR
            this._lblHR.Location = new System.Drawing.Point(12, 50);
            this._lblHR.Size = new System.Drawing.Size(236, 32);
            this._lblHR.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this._lblHR.ForeColor = System.Drawing.Color.FromArgb(220, 50, 50);
            this._lblHR.Text = "HR: --";
            this._lblHR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblSpO2
            this._lblSpO2.Location = new System.Drawing.Point(12, 84);
            this._lblSpO2.Size = new System.Drawing.Size(115, 28);
            this._lblSpO2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this._lblSpO2.ForeColor = System.Drawing.Color.FromArgb(0, 120, 200);
            this._lblSpO2.Text = "SpO2: --%";
            this._lblSpO2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblNIBP
            this._lblNIBP.Location = new System.Drawing.Point(132, 84);
            this._lblNIBP.Size = new System.Drawing.Size(116, 28);
            this._lblNIBP.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this._lblNIBP.ForeColor = System.Drawing.Color.FromArgb(80, 80, 200);
            this._lblNIBP.Text = "NIBP: --/--";
            this._lblNIBP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // sep
            sep.Location = new System.Drawing.Point(12, 130);
            sep.Size = new System.Drawing.Size(236, 1);
            sep.LineColor = System.Drawing.Color.FromArgb(230, 230, 230);

            // statusLight
            this._statusLight.Location = new System.Drawing.Point(14, 140);
            this._statusLight.Size = new System.Drawing.Size(12, 12);
            this._statusLight.State = Sunny.UI.UILightState.Off;
            this._statusLight.Radius = 6;

            // lblStatus
            this._lblStatus.Location = new System.Drawing.Point(30, 138);
            this._lblStatus.Size = new System.Drawing.Size(100, 18);
            this._lblStatus.Font = new System.Drawing.Font("Microsoft YaHei", 8F);
            this._lblStatus.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this._lblStatus.Text = "离线";

            // 控件尺寸
            this.Size = new System.Drawing.Size(260, 180);

            // 添加控件
            this.Controls.Add(this._lblBedNo);
            this.Controls.Add(this._lblPatientName);
            this.Controls.Add(this._lblPatientAge);
            this.Controls.Add(this._lblHR);
            this.Controls.Add(this._lblSpO2);
            this.Controls.Add(this._lblNIBP);
            this.Controls.Add(sep);
            this.Controls.Add(this._statusLight);
            this.Controls.Add(this._lblStatus);

            this.ResumeLayout(false);
        }


    }
}
