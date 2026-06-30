namespace MedicalMonitor.WinForm.UI
{
    partial class SetPatientsInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.setPatientsControl1 = new MedicalMonitor.WinForm.Controls.SetPatientsControl(_db);
            this.SuspendLayout();
            // 
            // setPatientsControl1
            // 
            this.setPatientsControl1.BackColor = System.Drawing.Color.Transparent;
            this.setPatientsControl1.FillColor = System.Drawing.SystemColors.ActiveCaption;
            this.setPatientsControl1.FillColor2 = System.Drawing.Color.Gray;
            this.setPatientsControl1.FillColorGradient = true;
            this.setPatientsControl1.FillColorGradientDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.setPatientsControl1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.setPatientsControl1.Location = new System.Drawing.Point(3, 38);
            this.setPatientsControl1.MinimumSize = new System.Drawing.Size(1, 1);
            this.setPatientsControl1.Name = "setPatientsControl1";
            this.setPatientsControl1.Size = new System.Drawing.Size(804, 494);
            this.setPatientsControl1.TabIndex = 0;
            this.setPatientsControl1.Text = "setPatientsControl1";
            this.setPatientsControl1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SetPatientsInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(810, 535);
            this.Controls.Add(this.setPatientsControl1);
            this.Name = "SetPatientsInfo";
            this.Text = "SetPatientsInfo";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 668, 438);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.SetPatientsControl setPatientsControl1;
    }
}