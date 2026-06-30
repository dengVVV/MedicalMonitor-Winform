namespace MedicalMonitor.WinForm.UI
{
    partial class DashboardForm
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

        /// <summary>
        /// 仅包含窗体本身属性和最外层 Panel 骨架。
        /// 子控件创建、循环生成的卡片、依赖运行时尺寸的布局计算，
        /// 全部移至主文件的 BuildLayout() 方法，在 InitializeComponent() 之后调用。
        /// </summary>
        private void InitializeComponent()
        {
            this._topToolbar = new Sunny.UI.UIPanel();
            this._gridViewPanel = new Sunny.UI.UIPanel();
            this._detailViewPanel = new Sunny.UI.UIPanel();
            this._statsBar = new Sunny.UI.UIPanel();

            this.SuspendLayout();

            // DashboardForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 820);
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name = "DashboardForm";
            this.Text = "医疗设备监控系统 — 多床位总览";
            this.ShowTitleIcon = true;

            // _topToolbar
            this._topToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this._topToolbar.Height = 48;
            this._topToolbar.FillColor = System.Drawing.Color.FromArgb(250, 251, 252);
            this._topToolbar.RectColor = System.Drawing.Color.FromArgb(220, 224, 230);
            this._topToolbar.Radius = 0;

            // _gridViewPanel
            this._gridViewPanel.Location = new System.Drawing.Point(240, 48);
            this._gridViewPanel.Size = new System.Drawing.Size(1160, 736);
            this._gridViewPanel.Anchor = System.Windows.Forms.AnchorStyles.Left
                                       | System.Windows.Forms.AnchorStyles.Top
                                       | System.Windows.Forms.AnchorStyles.Right
                                       | System.Windows.Forms.AnchorStyles.Bottom;
            this._gridViewPanel.FillColor = System.Drawing.Color.FromArgb(240, 243, 248);
            this._gridViewPanel.Radius = 0;

            // _detailViewPanel
            this._detailViewPanel.Location = new System.Drawing.Point(240, 48);
            this._detailViewPanel.Size = new System.Drawing.Size(1160, 736);
            this._detailViewPanel.Anchor = System.Windows.Forms.AnchorStyles.Left
                                         | System.Windows.Forms.AnchorStyles.Top
                                         | System.Windows.Forms.AnchorStyles.Right
                                         | System.Windows.Forms.AnchorStyles.Bottom;
            this._detailViewPanel.FillColor = System.Drawing.Color.FromArgb(240, 243, 248);
            this._detailViewPanel.Radius = 0;
            this._detailViewPanel.Visible = false;

            // _statsBar
            this._statsBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._statsBar.Height = 36;
            this._statsBar.FillColor = System.Drawing.Color.FromArgb(250, 251, 252);
            this._statsBar.RectColor = System.Drawing.Color.FromArgb(220, 224, 230);
            this._statsBar.Radius = 0;

            // Controls.Add（顶层四块面板，子内容运行时填充）
            this.Controls.Add(this._topToolbar);
            this.Controls.Add(this._gridViewPanel);
            this.Controls.Add(this._detailViewPanel);
            this.Controls.Add(this._statsBar);

            this.ResumeLayout(false);

            // 自定义控件（SidebarControl/BedCardControl/WaveformViewControl等）、
            // for循环生成的卡片、依赖ClientSize的动态布局，均不被设计器解析，移到运行时方法
            this.BuildLayout();
        }

        #endregion
    }
}