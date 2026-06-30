namespace MedicalMonitor.WinForm.UI
{
    partial class MainForm
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
            // 左侧面板
            this.LeftPanel = new Sunny.UI.UIPanel();
            this.lblSystemTitle = new Sunny.UI.UILabel();
            this.lblSystemSubtitle = new Sunny.UI.UILabel();
            this.lblSystemDesc = new Sunny.UI.UILabel();
            this.lblLeftCopyright = new Sunny.UI.UILabel();

            // 右侧面板
            this.RightPanel = new Sunny.UI.UIPanel();
            this.lblWelcome = new Sunny.UI.UILabel();
            this.lblWelcomeSub = new Sunny.UI.UILabel();
            this.txtUsername = new Sunny.UI.UITextBox();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.cmbRole = new Sunny.UI.UIComboBox();
            this.lblRole = new Sunny.UI.UILabel();
            this.btnLogin = new Sunny.UI.UIButton();
            this.chkRemember = new Sunny.UI.UICheckBox();
            this.lnkForgotPwd = new Sunny.UI.UILinkLabel();
            this.lblVersion = new Sunny.UI.UILabel();

            this.SuspendLayout();

            // ============================================================
            // LeftPanel
            // ============================================================
            this.LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPanel.Width = 380;
            this.LeftPanel.FillColor = System.Drawing.Color.FromArgb(0, 96, 176);
            this.LeftPanel.RectColor = System.Drawing.Color.Transparent;
            this.LeftPanel.Radius = 0;
            this.LeftPanel.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblSystemTitle
            // ============================================================
            this.lblSystemTitle.AutoSize = false;
            this.lblSystemTitle.Text = "医疗设备监控系统";
            this.lblSystemTitle.Font = new System.Drawing.Font("Microsoft YaHei", 22F, System.Drawing.FontStyle.Bold);
            this.lblSystemTitle.ForeColor = System.Drawing.Color.White;
            this.lblSystemTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSystemTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSystemTitle.Location = new System.Drawing.Point(0, 120);
            this.lblSystemTitle.Size = new System.Drawing.Size(380, 50);
            this.lblSystemTitle.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblSystemSubtitle
            // ============================================================
            this.lblSystemSubtitle.AutoSize = false;
            this.lblSystemSubtitle.Text = "Medical Device Monitoring System";
            this.lblSystemSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSystemSubtitle.ForeColor = System.Drawing.Color.FromArgb(180, 210, 240);
            this.lblSystemSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSystemSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSystemSubtitle.Location = new System.Drawing.Point(0, 175);
            this.lblSystemSubtitle.Size = new System.Drawing.Size(380, 30);
            this.lblSystemSubtitle.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblSystemDesc
            // ============================================================
            this.lblSystemDesc.AutoSize = false;
            this.lblSystemDesc.Text = "ICU · 手术室 · 普通病房\r\n实时生命体征监测 · 智能报警 · 数据追溯";
            this.lblSystemDesc.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.lblSystemDesc.ForeColor = System.Drawing.Color.FromArgb(200, 220, 245);
            this.lblSystemDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblSystemDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSystemDesc.Location = new System.Drawing.Point(0, 250);
            this.lblSystemDesc.Size = new System.Drawing.Size(380, 60);
            this.lblSystemDesc.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblLeftCopyright
            // ============================================================
            this.lblLeftCopyright.AutoSize = false;
            this.lblLeftCopyright.Text = "© 2026 Medical Monitor  v1.0.0";
            this.lblLeftCopyright.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblLeftCopyright.ForeColor = System.Drawing.Color.FromArgb(130, 170, 210);
            this.lblLeftCopyright.BackColor = System.Drawing.Color.Transparent;
            this.lblLeftCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLeftCopyright.Location = new System.Drawing.Point(0, 560);
            this.lblLeftCopyright.Size = new System.Drawing.Size(380, 30);
            this.lblLeftCopyright.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // RightPanel
            // ============================================================
            this.RightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightPanel.FillColor = System.Drawing.Color.White;
            this.RightPanel.RectColor = System.Drawing.Color.Transparent;
            this.RightPanel.Radius = 0;
            this.RightPanel.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblWelcome
            // ============================================================
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Text = "欢迎登录";
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft YaHei", 20F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWelcome.Location = new System.Drawing.Point(60, 80);
            this.lblWelcome.Size = new System.Drawing.Size(200, 40);
            this.lblWelcome.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // lblWelcomeSub
            // ============================================================
            this.lblWelcomeSub.AutoSize = false;
            this.lblWelcomeSub.Text = "请输入您的账号信息以进入系统";
            this.lblWelcomeSub.Font = new System.Drawing.Font("Microsoft YaHei", 9.5F);
            this.lblWelcomeSub.ForeColor = System.Drawing.Color.FromArgb(140, 140, 140);
            this.lblWelcomeSub.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcomeSub.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWelcomeSub.Location = new System.Drawing.Point(60, 120);
            this.lblWelcomeSub.Size = new System.Drawing.Size(300, 25);
            this.lblWelcomeSub.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // txtUsername
            // ============================================================
            this.txtUsername.Location = new System.Drawing.Point(60, 175);
            this.txtUsername.Size = new System.Drawing.Size(320, 42);
            this.txtUsername.Watermark = "用户名 / 工号";
            this.txtUsername.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.txtUsername.Radius = 6;
            this.txtUsername.RectColor = System.Drawing.Color.FromArgb(210, 215, 220);
            this.txtUsername.Style = Sunny.UI.UIStyle.Custom;
            this.txtUsername.FillColor = System.Drawing.Color.FromArgb(248, 249, 250);

            // ============================================================
            // txtPassword
            // ============================================================
            this.txtPassword.Location = new System.Drawing.Point(60, 235);
            this.txtPassword.Size = new System.Drawing.Size(320, 42);
            this.txtPassword.Watermark = "密码";
            this.txtPassword.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.txtPassword.PasswordChar = '\u25CF';
            this.txtPassword.Radius = 6;
            this.txtPassword.RectColor = System.Drawing.Color.FromArgb(210, 215, 220);
            this.txtPassword.Style = Sunny.UI.UIStyle.Custom;
            this.txtPassword.FillColor = System.Drawing.Color.FromArgb(248, 249, 250);

            // ============================================================
            // lblRole
            // ============================================================
            this.lblRole.AutoSize = false;
            this.lblRole.Text = "登录身份";
            this.lblRole.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.lblRole.BackColor = System.Drawing.Color.Transparent;
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRole.Location = new System.Drawing.Point(60, 290);
            this.lblRole.Size = new System.Drawing.Size(100, 22);
            this.lblRole.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // cmbRole
            // ============================================================
            this.cmbRole.Location = new System.Drawing.Point(60, 314);
            this.cmbRole.Size = new System.Drawing.Size(320, 38);
            this.cmbRole.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.cmbRole.Radius = 6;
            this.cmbRole.RectColor = System.Drawing.Color.FromArgb(210, 215, 220);
            this.cmbRole.Style = Sunny.UI.UIStyle.Custom;
            this.cmbRole.FillColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.cmbRole.Watermark = "请选择登录身份";
            this.cmbRole.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;

            // ============================================================
            // chkRemember
            // ============================================================
            this.chkRemember.Text = "记住密码";
            this.chkRemember.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.chkRemember.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.chkRemember.BackColor = System.Drawing.Color.Transparent;
            this.chkRemember.Location = new System.Drawing.Point(60, 370);
            this.chkRemember.Size = new System.Drawing.Size(100, 24);
            this.chkRemember.Style = Sunny.UI.UIStyle.Custom;
            this.chkRemember.Checked = false;

            // ============================================================
            // lnkForgotPwd
            // ============================================================
            this.lnkForgotPwd.Text = "忘记密码？";
            this.lnkForgotPwd.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.lnkForgotPwd.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lnkForgotPwd.BackColor = System.Drawing.Color.Transparent;
            this.lnkForgotPwd.LinkColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lnkForgotPwd.ActiveLinkColor = System.Drawing.Color.FromArgb(0, 150, 230);
            this.lnkForgotPwd.VisitedLinkColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lnkForgotPwd.AutoSize = true;
            this.lnkForgotPwd.Location = new System.Drawing.Point(310, 373);
            this.lnkForgotPwd.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkForgotPwd.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // btnLogin
            // ============================================================
            this.btnLogin.Text = "登  录";
            this.btnLogin.Font = new System.Drawing.Font("Microsoft YaHei", 13F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Location = new System.Drawing.Point(60, 420);
            this.btnLogin.Size = new System.Drawing.Size(320, 46);
            this.btnLogin.Radius = 6;
            this.btnLogin.Style = Sunny.UI.UIStyle.Custom;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnLogin.FillColor2 = System.Drawing.Color.FromArgb(0, 150, 230);
            this.btnLogin.FillHoverColor = System.Drawing.Color.FromArgb(0, 143, 220);
            this.btnLogin.FillPressColor = System.Drawing.Color.FromArgb(0, 100, 180);
            this.btnLogin.RectColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;

            // ============================================================
            // lblVersion
            // ============================================================
            this.lblVersion.AutoSize = false;
            this.lblVersion.Text = "Version 1.0.0  |  IEC 62304 Compliance";
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVersion.Location = new System.Drawing.Point(60, 560);
            this.lblVersion.Size = new System.Drawing.Size(320, 25);
            this.lblVersion.Style = Sunny.UI.UIStyle.Custom;

            // ============================================================
            // MainForm
            // ============================================================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Text = "医疗设备监控上位机系统";
            this.ShowTitleIcon = true;
            this.TitleFont = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.TitleColor = System.Drawing.Color.FromArgb(0, 96, 176);
            this.TitleHeight = 0;
            this.ShowRect = false;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 组装控件
            this.LeftPanel.Controls.Add(this.lblSystemTitle);
            this.LeftPanel.Controls.Add(this.lblSystemSubtitle);
            this.LeftPanel.Controls.Add(this.lblSystemDesc);
            this.LeftPanel.Controls.Add(this.lblLeftCopyright);

            this.RightPanel.Controls.Add(this.lblWelcome);
            this.RightPanel.Controls.Add(this.lblWelcomeSub);
            this.RightPanel.Controls.Add(this.txtUsername);
            this.RightPanel.Controls.Add(this.txtPassword);
            this.RightPanel.Controls.Add(this.lblRole);
            this.RightPanel.Controls.Add(this.cmbRole);
            this.RightPanel.Controls.Add(this.chkRemember);
            this.RightPanel.Controls.Add(this.lnkForgotPwd);
            this.RightPanel.Controls.Add(this.btnLogin);
            this.RightPanel.Controls.Add(this.lblVersion);

            this.Controls.Add(this.RightPanel);
            this.Controls.Add(this.LeftPanel);

            this.ResumeLayout(false);
        }

        #endregion

        // 左侧面板
        private Sunny.UI.UIPanel LeftPanel;
        private Sunny.UI.UILabel lblSystemTitle;
        private Sunny.UI.UILabel lblSystemSubtitle;
        private Sunny.UI.UILabel lblSystemDesc;
        private Sunny.UI.UILabel lblLeftCopyright;

        // 右侧面板
        private Sunny.UI.UIPanel RightPanel;
        private Sunny.UI.UILabel lblWelcome;
        private Sunny.UI.UILabel lblWelcomeSub;
        private Sunny.UI.UITextBox txtUsername;
        private Sunny.UI.UITextBox txtPassword;
        private Sunny.UI.UIComboBox cmbRole;
        private Sunny.UI.UILabel lblRole;
        private Sunny.UI.UIButton btnLogin;
        private Sunny.UI.UICheckBox chkRemember;
        private Sunny.UI.UILinkLabel lnkForgotPwd;
        private Sunny.UI.UILabel lblVersion;
    }
}
