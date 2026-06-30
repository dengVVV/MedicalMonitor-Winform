using MedicalMonitor.WinForm.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    public partial class SidebarControl : UIUserControl
    {
        private const int ExpandedWidth = 240, CollapsedWidth = 48;
        private bool _isExpanded = true;

        private UIButton _btnToggle;
        private UILabel _lblTitle, _lblAlarmHeader, _lblAlarmContent, _lblNavHeader, _lblUserInfo;
        private UIPanel _patientListPanel;
        private UIListBox _navListBox;
        private UISymbolButton _btnLogout;
        private readonly List<UILabel> _bedItems = new List<UILabel>();

        public event EventHandler<string> BedClicked;
        public event EventHandler<string> NavClicked;
        public event EventHandler LogoutClicked;

        public SidebarControl()
        {
            InitializeComponent();
            _btnToggle.Click += (s, e) => ToggleExpand();
            _btnLogout.Click += (s, e) => LogoutClicked?.Invoke(this, EventArgs.Empty);
            _navListBox.SelectedIndexChanged += OnNavItemClicked;
            foreach (var item in _bedItems)
            {
                item.Click += OnBedItemClick;
                item.MouseEnter += (s, e) => { if (s is UILabel lbl) lbl.BackColor = Color.FromArgb(230, 240, 255); };
                item.MouseLeave += (s, e) => { if (s is UILabel lbl) lbl.BackColor = Color.Transparent; };
            }
        }

        private void ToggleExpand()
        {
            _isExpanded = !_isExpanded;
            this.Width = _isExpanded ? ExpandedWidth : CollapsedWidth;
            _lblTitle.Visible = _patientListPanel.Visible = _lblAlarmHeader.Visible = _lblAlarmContent.Visible = _lblNavHeader.Visible = _navListBox.Visible = _lblUserInfo.Visible = _btnLogout.Visible = _isExpanded;
        }

        public void UpdateAlarmCount(int count)
        {
            if (_lblAlarmContent != null)
            {
                _lblAlarmContent.Text = count > 0
                    ? $"🔔 {count} 条未确认报警"
                    : "🔔 无报警";
                _lblAlarmContent.ForeColor = count > 0
                    ? System.Drawing.Color.Red
                    : System.Drawing.Color.FromArgb(100, 100, 100);
            }
        }

        public void SetOperator(string fullName, string role) => _lblUserInfo.Text = $"👤 {fullName}\n   {role}";

        public void UpdateBedItem(string bedNo, string patientName, bool isOnline)
        {
            foreach (var item in _bedItems)
                if ((string)item.Tag == bedNo)
                {
                    item.Text = $"  {(isOnline ? "●" : "○")} {bedNo}  在线";
                    item.ForeColor = isOnline ? Color.FromArgb(50, 50, 50) : Color.FromArgb(180, 180, 180);
                    break;
                }
        }

        private void OnBedItemClick(object sender, EventArgs e) 
        { 
            if (sender is UILabel item && item.Tag is string b) 
                BedClicked?.Invoke(this, b); 
        
        
        }

        private void OnNavItemClicked(object sender, EventArgs e) 
        { 
            if (_navListBox.SelectedItem is string n)
                NavClicked?.Invoke(this, n.Trim('▸', ' ')); 
        
        
        }

        private void InitializeBedItems()
        {
            for (int i = 1; i <= 8; i++)
            {
                var item = new UILabel
                {
                    Text = $"  ○ BED-{i:D2}  离线",
                    Location = new Point(0, (i - 1) * 38),
                    Size = new Size(218, 36),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Microsoft YaHei", 9),
                    ForeColor = Color.FromArgb(180, 180, 180),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    Tag = $"BED-{i:D2}",
                    AutoSize = false
                };
                _bedItems.Add(item);
                _patientListPanel.Controls.Add(item);
            }
        }
    }
}
