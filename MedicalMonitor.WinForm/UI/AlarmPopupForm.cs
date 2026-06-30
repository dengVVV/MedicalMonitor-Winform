using MedicalMonitor.WinForm.BLL;
using MedicalMonitor.WinForm.Models;
using MedicalMonitor.WinForm.Models.Enums;
using Sunny.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    /// <summary>
    /// 报警弹窗 — 非模态窗口，多个报警各自独立弹出。
    /// 显示患者信息、报警参数、等级，支持确认操作。
    /// </summary>
    public partial class AlarmPopupForm : UIForm
    {
        private readonly AlarmEventModel _alarm;
        private readonly AlarmDetectionService _alarmService;
        private readonly string _operatorName;

        private UILabel _lblTitle, _lblPatient, _lblParameter, _lblTime;
        private UIButton _btnAcknowledge;
        private System.Timers.Timer _blinkTimer;
        private bool _blinkState;

        private static bool IsDesignTime =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public AlarmPopupForm() { InitializeComponent(); }

        public AlarmPopupForm(AlarmEventModel alarm, AlarmDetectionService alarmService, string operatorName)
            : this()
        {
            _alarm = alarm;
            _alarmService = alarmService;
            _operatorName = operatorName;

            SetupContent();
        }

        private void SetupContent()
        {
            if (_alarm == null) return;

            // 根据报警等级设置背景色和闪烁
            Color bgColor;
            switch (_alarm.Level)
            {
                case AlarmLevel.High: bgColor = Color.FromArgb(255, 230, 230); break;
                case AlarmLevel.Medium: bgColor = Color.FromArgb(255, 248, 220); break;
                default: bgColor = Color.FromArgb(255, 255, 240); break;
            }

            this.BackColor = bgColor;
            this.RectColor = _alarm.Level == AlarmLevel.High ? Color.Red : Color.Orange;

            string levelText;
            switch (_alarm.Level)
            {
                case AlarmLevel.High: levelText = "高级报警"; break;
                case AlarmLevel.Medium: levelText = "中级报警"; break;
                default: levelText = "低级报警"; break;
            }

            _lblTitle.Text = levelText;
            _lblPatient.Text = $"患者: {GetPatientDisplayName()}";
            _lblParameter.Text = _alarm.Message;
            _lblTime.Text = $"触发时间: {_alarm.TriggerTime:HH:mm:ss}";

            // 高级报警：闪烁效果
            if (_alarm.Level == AlarmLevel.High)
            {
                _blinkTimer = new System.Timers.Timer(500);
                _blinkTimer.AutoReset = true;
                _blinkTimer.Elapsed += (s, e) =>
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            _blinkState = !_blinkState;
                            this.BackColor = _blinkState ? Color.FromArgb(255, 200, 200) : Color.FromArgb(255, 230, 230);
                        }));
                    }
                };
                _blinkTimer.Start();
            }

            _btnAcknowledge.Click += (s, e) =>
            {
                _alarmService?.AcknowledgeAlarm(_alarm.AlarmId, _operatorName);
                _blinkTimer?.Stop();
                this.Close();
            };
        }

        private string GetPatientDisplayName()
        {
            if (string.IsNullOrEmpty(_alarm.PatientId)) return _alarm.DeviceId;
            return $"{_alarm.PatientId} ({_alarm.DeviceId})";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _blinkTimer?.Stop();
            _blinkTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
