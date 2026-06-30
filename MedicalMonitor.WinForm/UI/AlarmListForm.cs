using MedicalMonitor.WinForm.BLL;
using MedicalMonitor.WinForm.DAL.Entities;
using Sunny.UI;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    /// <summary>
    /// 报警列表窗口 — 展示所有报警记录，支持筛选和确认。
    /// </summary>
    public partial class AlarmListForm : UIForm
    {
        private readonly AlarmDetectionService _alarmService;
        private readonly string _operatorName;

        private DataGridView _dgvAlarms;
        private UIButton _btnRefresh, _btnAcknowledge;
        private UIComboBox _cmbFilter;
        private UILabel _lblCount;
        private UIPanel _topPanel;

        private List<AlarmEventEntity> _allAlarms;

        private static bool IsDesignTime =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public AlarmListForm() { InitializeComponent(); }

        public AlarmListForm(AlarmDetectionService alarmService, string operatorName) : this()
        {
            _alarmService = alarmService;
            _operatorName = operatorName;

            _btnRefresh.Click += (s, e) => LoadAlarms();
            _btnAcknowledge.Click += (s, e) => AcknowledgeSelected();

            this.Load += (s, e) => LoadAlarms();
        }

        private void LoadAlarms()
        {
            if (_alarmService == null) return;

            string filter = _cmbFilter?.SelectedItem?.ToString() ?? "全部";
            bool? acknowledged = null;
            switch (filter)
            {
                case "未确认": acknowledged = false; break;
                case "已确认": acknowledged = true; break;
            }

            var (items, total) = _alarmService.QueryPaged(1, 500, acknowledged);
            _allAlarms = items;
            _lblCount.Text = $"共 {total} 条报警";
            BindGrid();
        }

        private void BindGrid()
        {
            _dgvAlarms.Rows.Clear();
            if (_allAlarms == null) return;

            foreach (var a in _allAlarms)
            {
                string levelText;
                switch (a.AlarmLevel) { case 1: levelText = "高"; break; case 2: levelText = "中"; break; case 3: levelText = "低"; break; default: levelText = "?"; break; }
                string status = a.AcknowledgedTime.HasValue ? "已确认" : "⚠ 未确认";
                _dgvAlarms.Rows.Add(a.AlarmId, a.DeviceId, a.PatientId, a.Message,
                    levelText, a.TriggerTime.ToString("MM-dd HH:mm:ss"), status, a.AcknowledgedBy ?? "");
            }
        }

        private void AcknowledgeSelected()
        {
            if (_dgvAlarms.SelectedRows.Count == 0)
            {
                UIMessageBox.ShowWarning("请先选择要确认的报警");
                return;
            }

            foreach (DataGridViewRow row in _dgvAlarms.SelectedRows)
            {
                string alarmId = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(alarmId))
                    _alarmService?.AcknowledgeAlarm(alarmId, _operatorName);
            }
            LoadAlarms();
            UIMessageBox.ShowSuccess("已确认选中报警");
        }
    }
}
