namespace MedicalMonitor.WinForm.UI
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 580);
            this.Name = "SettingsForm";
            this.Text = "系统设置";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowTitleIcon = true;

            int y = 20;

            var lblTitle = new Sunny.UI.UILabel();
            lblTitle.Text = "生理参数报警阈值配置";
            lblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(45, 120, 210);
            lblTitle.Location = new System.Drawing.Point(20, y);
            lblTitle.Size = new System.Drawing.Size(400, 28);
            this.Controls.Add(lblTitle);
            y += 40;

            this._txtHRLow = CreateRow("心率 (HR)", "下限 bpm", ref y);
            this._txtHRHigh = CreateRow("", "上限 bpm", ref y);
            y += 8;
            this._txtSpO2Low = CreateRow("血氧 (SpO2)", "下限 %", ref y);
            this._txtSpO2High = CreateRow("", "上限 %", ref y);
            y += 8;
            this._txtSystolicLow = CreateRow("收缩压", "下限 mmHg", ref y);
            this._txtSystolicHigh = CreateRow("", "上限 mmHg", ref y);
            y += 8;
            this._txtDiastolicLow = CreateRow("舒张压", "下限 mmHg", ref y);
            this._txtDiastolicHigh = CreateRow("", "上限 mmHg", ref y);
            y += 8;
            this._txtRESPLow = CreateRow("呼吸频率 (RESP)", "下限 次/分", ref y);
            this._txtRESPHigh = CreateRow("", "上限 次/分", ref y);
            y += 8;
            this._txtTEMPLow = CreateRow("体温 (TEMP)", "下限 ℃", ref y);
            this._txtTEMPHigh = CreateRow("", "上限 ℃", ref y);
            y += 20;

            var btnSave = new Sunny.UI.UIButton();
            btnSave.Text = "保存设置";
            btnSave.Location = new System.Drawing.Point(180, y);
            btnSave.Size = new System.Drawing.Size(140, 38);
            btnSave.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            btnSave.FillColor = System.Drawing.Color.FromArgb(45, 120, 210);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.RectColor = System.Drawing.Color.Transparent;
            btnSave.Radius = 6;
            btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            btnSave.Click += (s, e) => SaveThresholds();
            this.Controls.Add(btnSave);

            this.ResumeLayout(false);
        }

        private Sunny.UI.UITextBox CreateRow(string labelText, string fieldLabel, ref int y)
        {
            if (!string.IsNullOrEmpty(labelText))
            {
                var lbl = new Sunny.UI.UILabel();
                lbl.Text = labelText;
                lbl.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
                lbl.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
                lbl.Location = new System.Drawing.Point(30, y);
                lbl.Size = new System.Drawing.Size(160, 22);
                this.Controls.Add(lbl);
            }
            var txt = new Sunny.UI.UITextBox();
            txt.Watermark = fieldLabel;
            txt.Font = new System.Drawing.Font("Consolas", 10F);
            txt.Location = new System.Drawing.Point(220, y);
            txt.Size = new System.Drawing.Size(120, 26);
            this.Controls.Add(txt);
            y += 32;
            return txt;
        }

        #endregion
    }
}
