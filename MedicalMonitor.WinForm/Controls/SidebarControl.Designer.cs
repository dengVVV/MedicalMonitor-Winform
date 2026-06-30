using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    partial class SidebarControl
    {
        private void InitializeComponent()
        {
            this._btnToggle = new UIButton();
            this._lblTitle = new UILabel();
            this._patientListPanel = new UIPanel();
            UILine sep1 = new UILine();
            this._lblAlarmHeader = new UILabel();
            this._lblAlarmContent = new UILabel();
            UILine sep2 = new UILine();
            this._lblNavHeader = new UILabel();
            this._navListBox = new UIListBox();
            this._lblUserInfo = new UILabel();
            this._btnLogout = new UISymbolButton();

            this.SuspendLayout();

            // SidebarControl
            this.Size = new System.Drawing.Size(ExpandedWidth, 700);
            this.FillColor = System.Drawing.Color.FromArgb(245, 248, 252);
            this.RectColor = System.Drawing.Color.FromArgb(210, 215, 225);
            this.Radius = 0;

            // _btnToggle
            this._btnToggle.Location = new System.Drawing.Point(5, 5);
            this._btnToggle.Size = new System.Drawing.Size(36, 30);
            this._btnToggle.Text = "☰";
            this._btnToggle.Font = new System.Drawing.Font("Microsoft YaHei", 14F);
            this._btnToggle.FillColor = System.Drawing.Color.Transparent;
            this._btnToggle.RectColor = System.Drawing.Color.Transparent;
            this._btnToggle.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this._btnToggle.Radius = 5;
            this._btnToggle.Cursor = System.Windows.Forms.Cursors.Hand;

            // _lblTitle
            this._lblTitle.Location = new System.Drawing.Point(46, 9);
            this._lblTitle.Size = new System.Drawing.Size(180, 22);
            this._lblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold);
            this._lblTitle.ForeColor = System.Drawing.Color.FromArgb(45, 120, 210);
            this._lblTitle.Text = "📋 患者列表";

            // _patientListPanel
            this._patientListPanel.Location = new System.Drawing.Point(5, 45);
            this._patientListPanel.Size = new System.Drawing.Size(230, 360);
            this._patientListPanel.FillColor = System.Drawing.Color.Transparent;
            this._patientListPanel.RectColor = System.Drawing.Color.Transparent;
            this._patientListPanel.AutoScroll = true;

            // sep1
            sep1.Location = new System.Drawing.Point(10, 415);
            sep1.Size = new System.Drawing.Size(220, 1);
            sep1.LineColor = System.Drawing.Color.FromArgb(210, 215, 225);

            // _lblAlarmHeader
            this._lblAlarmHeader.Location = new System.Drawing.Point(12, 425);
            this._lblAlarmHeader.Size = new System.Drawing.Size(200, 20);
            this._lblAlarmHeader.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this._lblAlarmHeader.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this._lblAlarmHeader.Text = "🔔 报警摘要";

            // _lblAlarmContent
            this._lblAlarmContent.Location = new System.Drawing.Point(12, 449);
            this._lblAlarmContent.Size = new System.Drawing.Size(210, 20);
            this._lblAlarmContent.Font = new System.Drawing.Font("Microsoft YaHei", 8F);
            this._lblAlarmContent.ForeColor = System.Drawing.Color.FromArgb(150, 150, 150);
            this._lblAlarmContent.Text = "暂无报警";

            // sep2
            sep2.Location = new System.Drawing.Point(10, 485);
            sep2.Size = new System.Drawing.Size(220, 1);
            sep2.LineColor = System.Drawing.Color.FromArgb(210, 215, 225);

            // _lblNavHeader
            this._lblNavHeader.Location = new System.Drawing.Point(12, 495);
            this._lblNavHeader.Size = new System.Drawing.Size(200, 20);
            this._lblNavHeader.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            this._lblNavHeader.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this._lblNavHeader.Text = "📊 导航";

            // _navListBox
            this._navListBox.Location = new System.Drawing.Point(10, 519);
            this._navListBox.Size = new System.Drawing.Size(215, 130);
            this._navListBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this._navListBox.FillColor = System.Drawing.Color.White;
            this._navListBox.RectColor = System.Drawing.Color.FromArgb(210, 215, 225);
            this._navListBox.HoverColor = System.Drawing.Color.FromArgb(225, 240, 255);
            this._navListBox.ItemSelectBackColor = System.Drawing.Color.FromArgb(45, 120, 210);
            this._navListBox.ItemSelectForeColor = System.Drawing.Color.White;
            this._navListBox.Radius = 6;
            this._navListBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._navListBox.Items.Add("▸ 监控总览");
            this._navListBox.Items.Add("  患者管理");
            this._navListBox.Items.Add("  历史数据");
            this._navListBox.Items.Add("  系统设置");
            this._navListBox.SelectedIndex = 0;

            // _lblUserInfo
            this._lblUserInfo.Location = new System.Drawing.Point(12, 659);
            this._lblUserInfo.Size = new System.Drawing.Size(210, 36);
            this._lblUserInfo.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this._lblUserInfo.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this._lblUserInfo.Text = "👤 未登录";
            this._lblUserInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // _btnLogout
            this._btnLogout.Location = new System.Drawing.Point(170, 659);
            this._btnLogout.Size = new System.Drawing.Size(55, 34);
            this._btnLogout.Symbol = 61457;
            this._btnLogout.SymbolSize = 16;
            this._btnLogout.Text = "退出";
            this._btnLogout.Font = new System.Drawing.Font("Microsoft YaHei", 8F);
            this._btnLogout.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            this._btnLogout.ForeColor = System.Drawing.Color.White;
            this._btnLogout.RectColor = System.Drawing.Color.Transparent;
            this._btnLogout.Radius = 5;
            this._btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;

            // Controls.Add
            this.Controls.Add(this._btnToggle);
            this.Controls.Add(this._lblTitle);
            this.Controls.Add(this._patientListPanel);
            this.Controls.Add(sep1);
            this.Controls.Add(this._lblAlarmHeader);
            this.Controls.Add(this._lblAlarmContent);
            this.Controls.Add(sep2);
            this.Controls.Add(this._lblNavHeader);
            this.Controls.Add(this._navListBox);
            this.Controls.Add(this._lblUserInfo);
            this.Controls.Add(this._btnLogout);

            this.ResumeLayout(false);

            // 床位列表项在运行时动态生成，设计器不解析
            this.InitializeBedItems();
        }
    }
}