using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    partial class AlarmListForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._topPanel = new UIPanel();
            this._btnRefresh = new UIButton();
            this._btnAcknowledge = new UIButton();
            this._cmbFilter = new UIComboBox();
            this._lblCount = new UILabel();
            this._dgvAlarms = new DataGridView();

            this.SuspendLayout();

            // AlarmListForm
            this.ClientSize = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "报警管理";
            this.ShowTitleIcon = true;

            // _topPanel
            this._topPanel.Dock = DockStyle.Top;
            this._topPanel.Height = 50;
            this._topPanel.FillColor = Color.FromArgb(250, 251, 252);
            this._topPanel.RectColor = Color.FromArgb(220, 224, 230);
            this._topPanel.Radius = 0;

            // _cmbFilter
            this._cmbFilter.Location = new Point(15, 12);
            this._cmbFilter.Size = new Size(120, 28);
            this._cmbFilter.Items.AddRange(new object[] { "全部", "未确认", "已确认" });
            this._cmbFilter.SelectedIndex = 1;
            this._cmbFilter.DropDownStyle = UIDropDownStyle.DropDownList;

            // _btnRefresh
            this._btnRefresh.Location = new Point(145, 10);
            this._btnRefresh.Size = new Size(80, 32);
            this._btnRefresh.Text = "刷新";
            this._btnRefresh.Style = UIStyle.Blue;

            // _btnAcknowledge
            this._btnAcknowledge.Location = new Point(235, 10);
            this._btnAcknowledge.Size = new Size(100, 32);
            this._btnAcknowledge.Text = "确认选中";
            this._btnAcknowledge.Style = UIStyle.Blue;

            // _lblCount
            this._lblCount.Location = new Point(700, 14);
            this._lblCount.Size = new Size(180, 24);
            this._lblCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._lblCount.Text = "共 0 条报警";
            this._lblCount.TextAlign = ContentAlignment.MiddleRight;
            this._lblCount.Font = new Font("Microsoft YaHei", 10);
            this._lblCount.ForeColor = Color.FromArgb(100, 100, 100);
            this._lblCount.BackColor = Color.Transparent;
            this._lblCount.AutoSize = false;

            this._topPanel.Controls.AddRange(new Control[] { _cmbFilter, _btnRefresh, _btnAcknowledge, _lblCount });

            // _dgvAlarms
            this._dgvAlarms.Location = new Point(0, 50);
            this._dgvAlarms.Size = new Size(900, 500);
            this._dgvAlarms.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this._dgvAlarms.AllowUserToAddRows = false;
            this._dgvAlarms.AllowUserToDeleteRows = false;
            this._dgvAlarms.ReadOnly = true;
            this._dgvAlarms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._dgvAlarms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvAlarms.BackgroundColor = Color.White;
            this._dgvAlarms.RowHeadersVisible = false;

            // Columns
            this._dgvAlarms.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "AlarmId", HeaderText = "AlarmId", Visible = false },
                new DataGridViewTextBoxColumn { Name = "DeviceId", HeaderText = "设备", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "PatientId", HeaderText = "患者ID", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Message", HeaderText = "报警内容", Width = 220 },
                new DataGridViewTextBoxColumn { Name = "Level", HeaderText = "等级", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "TriggerTime", HeaderText = "触发时间", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "状态", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "AcknowledgedBy", HeaderText = "确认人", Width = 80 },
            });

            // Form Controls
            this.Controls.AddRange(new Control[] { _topPanel, _dgvAlarms });

            this.ResumeLayout(false);
        }
    }
}
