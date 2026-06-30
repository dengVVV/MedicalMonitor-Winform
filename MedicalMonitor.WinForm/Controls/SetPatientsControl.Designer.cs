namespace MedicalMonitor.WinForm.Controls
{
    partial class SetPatientsControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Control_Title = new Sunny.UI.UILabel();
            this.Patient_Name = new Sunny.UI.UILabel();
            this.Patient_BedNo = new Sunny.UI.UILabel();
            this.Patient_Age = new Sunny.UI.UILabel();
            this.Patient_Sex = new Sunny.UI.UILabel();
            this.Patient_Diagnosis = new Sunny.UI.UILabel();
            this.Patient_AdmissionTime = new Sunny.UI.UILabel();
            this.PatientName_Value = new Sunny.UI.UITextBox();
            this.PatinetAge_Value = new Sunny.UI.UITextBox();
            this.AdmissionTime_Value = new Sunny.UI.UITextBox();
            this.PatientBed_Value = new Sunny.UI.UITextBox();
            this.PatientGender_Value = new Sunny.UI.UITextBox();
            this.Diagnosis_Value = new Sunny.UI.UITextBox();
            this.AddFinish_btn = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // Control_Title
            // 
            this.Control_Title.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Control_Title.BackColor = System.Drawing.Color.Transparent;
            this.Control_Title.Font = new System.Drawing.Font("微软雅黑", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Control_Title.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Control_Title.Location = new System.Drawing.Point(227, 11);
            this.Control_Title.Name = "Control_Title";
            this.Control_Title.Size = new System.Drawing.Size(214, 35);
            this.Control_Title.TabIndex = 0;
            this.Control_Title.Text = "病人信息";
            this.Control_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Patient_Name
            // 
            this.Patient_Name.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_Name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_Name.Location = new System.Drawing.Point(92, 84);
            this.Patient_Name.Name = "Patient_Name";
            this.Patient_Name.Size = new System.Drawing.Size(71, 27);
            this.Patient_Name.TabIndex = 1;
            this.Patient_Name.Text = "患者姓名";
            this.Patient_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Patient_BedNo
            // 
            this.Patient_BedNo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_BedNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_BedNo.Location = new System.Drawing.Point(397, 81);
            this.Patient_BedNo.Name = "Patient_BedNo";
            this.Patient_BedNo.Size = new System.Drawing.Size(68, 33);
            this.Patient_BedNo.TabIndex = 2;
            this.Patient_BedNo.Text = "患者床位";
            this.Patient_BedNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Patient_Age
            // 
            this.Patient_Age.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_Age.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_Age.Location = new System.Drawing.Point(92, 155);
            this.Patient_Age.Name = "Patient_Age";
            this.Patient_Age.Size = new System.Drawing.Size(71, 33);
            this.Patient_Age.TabIndex = 3;
            this.Patient_Age.Text = "患者年龄";
            this.Patient_Age.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Patient_Sex
            // 
            this.Patient_Sex.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_Sex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_Sex.Location = new System.Drawing.Point(397, 145);
            this.Patient_Sex.Name = "Patient_Sex";
            this.Patient_Sex.Size = new System.Drawing.Size(68, 33);
            this.Patient_Sex.TabIndex = 4;
            this.Patient_Sex.Text = "患者性别";
            this.Patient_Sex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Patient_Diagnosis
            // 
            this.Patient_Diagnosis.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_Diagnosis.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_Diagnosis.Location = new System.Drawing.Point(3, 268);
            this.Patient_Diagnosis.Name = "Patient_Diagnosis";
            this.Patient_Diagnosis.Size = new System.Drawing.Size(93, 33);
            this.Patient_Diagnosis.TabIndex = 5;
            this.Patient_Diagnosis.Text = "医生诊断结果";
            this.Patient_Diagnosis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Patient_AdmissionTime
            // 
            this.Patient_AdmissionTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Patient_AdmissionTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Patient_AdmissionTime.Location = new System.Drawing.Point(92, 218);
            this.Patient_AdmissionTime.Name = "Patient_AdmissionTime";
            this.Patient_AdmissionTime.Size = new System.Drawing.Size(71, 22);
            this.Patient_AdmissionTime.TabIndex = 6;
            this.Patient_AdmissionTime.Text = "入院时间";
            this.Patient_AdmissionTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PatientName_Value
            // 
            this.PatientName_Value.BackColor = System.Drawing.Color.Transparent;
            this.PatientName_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PatientName_Value.Location = new System.Drawing.Point(177, 85);
            this.PatientName_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PatientName_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.PatientName_Value.Name = "PatientName_Value";
            this.PatientName_Value.Padding = new System.Windows.Forms.Padding(5);
            this.PatientName_Value.ShowText = false;
            this.PatientName_Value.Size = new System.Drawing.Size(141, 23);
            this.PatientName_Value.TabIndex = 7;
            this.PatientName_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.PatientName_Value.Watermark = "";
            // 
            // PatinetAge_Value
            // 
            this.PatinetAge_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PatinetAge_Value.Location = new System.Drawing.Point(177, 155);
            this.PatinetAge_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PatinetAge_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.PatinetAge_Value.Name = "PatinetAge_Value";
            this.PatinetAge_Value.Padding = new System.Windows.Forms.Padding(5);
            this.PatinetAge_Value.ShowText = false;
            this.PatinetAge_Value.Size = new System.Drawing.Size(141, 23);
            this.PatinetAge_Value.TabIndex = 8;
            this.PatinetAge_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.PatinetAge_Value.Watermark = "";
            // 
            // AdmissionTime_Value
            // 
            this.AdmissionTime_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AdmissionTime_Value.Location = new System.Drawing.Point(177, 217);
            this.AdmissionTime_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AdmissionTime_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.AdmissionTime_Value.Name = "AdmissionTime_Value";
            this.AdmissionTime_Value.Padding = new System.Windows.Forms.Padding(5);
            this.AdmissionTime_Value.ShowText = false;
            this.AdmissionTime_Value.Size = new System.Drawing.Size(141, 23);
            this.AdmissionTime_Value.TabIndex = 8;
            this.AdmissionTime_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.AdmissionTime_Value.Watermark = "";
            // 
            // PatientBed_Value
            // 
            this.PatientBed_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PatientBed_Value.Location = new System.Drawing.Point(472, 85);
            this.PatientBed_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PatientBed_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.PatientBed_Value.Name = "PatientBed_Value";
            this.PatientBed_Value.Padding = new System.Windows.Forms.Padding(5);
            this.PatientBed_Value.ShowText = false;
            this.PatientBed_Value.Size = new System.Drawing.Size(141, 23);
            this.PatientBed_Value.TabIndex = 8;
            this.PatientBed_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.PatientBed_Value.Watermark = "";
            // 
            // PatientGender_Value
            // 
            this.PatientGender_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PatientGender_Value.Location = new System.Drawing.Point(472, 155);
            this.PatientGender_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PatientGender_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.PatientGender_Value.Name = "PatientGender_Value";
            this.PatientGender_Value.Padding = new System.Windows.Forms.Padding(5);
            this.PatientGender_Value.ShowText = false;
            this.PatientGender_Value.Size = new System.Drawing.Size(141, 23);
            this.PatientGender_Value.TabIndex = 8;
            this.PatientGender_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.PatientGender_Value.Watermark = "";
            // 
            // Diagnosis_Value
            // 
            this.Diagnosis_Value.FillColor2 = System.Drawing.Color.Gainsboro;
            this.Diagnosis_Value.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Diagnosis_Value.ForeColor = System.Drawing.Color.Black;
            this.Diagnosis_Value.Location = new System.Drawing.Point(127, 276);
            this.Diagnosis_Value.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Diagnosis_Value.MinimumSize = new System.Drawing.Size(1, 16);
            this.Diagnosis_Value.Multiline = true;
            this.Diagnosis_Value.Name = "Diagnosis_Value";
            this.Diagnosis_Value.Padding = new System.Windows.Forms.Padding(5);
            this.Diagnosis_Value.ScrollBarWidth = 1;
            this.Diagnosis_Value.ShowScrollBar = true;
            this.Diagnosis_Value.ShowText = false;
            this.Diagnosis_Value.Size = new System.Drawing.Size(412, 189);
            this.Diagnosis_Value.TabIndex = 9;
            this.Diagnosis_Value.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.Diagnosis_Value.Watermark = "";
            // 
            // AddFinish_btn
            // 
            this.AddFinish_btn.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.AddFinish_btn.FillColor = System.Drawing.SystemColors.HotTrack;
            this.AddFinish_btn.FillColor2 = System.Drawing.SystemColors.AppWorkspace;
            this.AddFinish_btn.FillColorGradient = true;
            this.AddFinish_btn.FillColorGradientDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.AddFinish_btn.FillDisableColor = System.Drawing.Color.Gray;
            this.AddFinish_btn.FillHoverColor = System.Drawing.Color.Lime;
            this.AddFinish_btn.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.AddFinish_btn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddFinish_btn.ForeColor = System.Drawing.Color.Black;
            this.AddFinish_btn.Location = new System.Drawing.Point(568, 423);
            this.AddFinish_btn.MinimumSize = new System.Drawing.Size(1, 1);
            this.AddFinish_btn.Name = "AddFinish_btn";
            this.AddFinish_btn.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(179)))), ((int)(((byte)(255)))));
            this.AddFinish_btn.Size = new System.Drawing.Size(89, 36);
            this.AddFinish_btn.TabIndex = 10;
            this.AddFinish_btn.Text = "确定";
            this.AddFinish_btn.TipsColor = System.Drawing.Color.LawnGreen;
            this.AddFinish_btn.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // SetPatientsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.AddFinish_btn);
            this.Controls.Add(this.Diagnosis_Value);
            this.Controls.Add(this.PatientGender_Value);
            this.Controls.Add(this.PatientBed_Value);
            this.Controls.Add(this.AdmissionTime_Value);
            this.Controls.Add(this.PatinetAge_Value);
            this.Controls.Add(this.PatientName_Value);
            this.Controls.Add(this.Patient_AdmissionTime);
            this.Controls.Add(this.Patient_Diagnosis);
            this.Controls.Add(this.Patient_Sex);
            this.Controls.Add(this.Patient_Age);
            this.Controls.Add(this.Patient_BedNo);
            this.Controls.Add(this.Patient_Name);
            this.Controls.Add(this.Control_Title);
            this.FillColor = System.Drawing.SystemColors.ActiveCaption;
            this.FillColor2 = System.Drawing.Color.Gray;
            this.FillColorGradient = true;
            this.FillColorGradientDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.Name = "SetPatientsControl";
            this.Size = new System.Drawing.Size(684, 477);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILabel Control_Title;
        private Sunny.UI.UILabel Patient_Name;
        private Sunny.UI.UILabel Patient_BedNo;
        private Sunny.UI.UILabel Patient_Age;
        private Sunny.UI.UILabel Patient_Sex;
        private Sunny.UI.UILabel Patient_Diagnosis;
        private Sunny.UI.UILabel Patient_AdmissionTime;
        private Sunny.UI.UITextBox PatientName_Value;
        private Sunny.UI.UITextBox PatinetAge_Value;
        private Sunny.UI.UITextBox AdmissionTime_Value;
        private Sunny.UI.UITextBox PatientBed_Value;
        private Sunny.UI.UITextBox PatientGender_Value;
        private Sunny.UI.UITextBox Diagnosis_Value;
        private Sunny.UI.UIButton AddFinish_btn;
    }
}
