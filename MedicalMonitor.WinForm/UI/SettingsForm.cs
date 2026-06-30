using MedicalMonitor.WinForm.BLL;
using Sunny.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    /// <summary>
    /// SettingsForm — 企业级 WinForms 规范：
    ///   所有控件创建在 Designer.cs 的 InitializeComponent() 中。
    ///   构造函数只做 InitializeComponent() + 事件绑定。
    /// </summary>
    public partial class SettingsForm : UIForm
    {
        private readonly ThresholdConfigService _thresholdConfig;

        // 控件字段（由 Designer.cs 创建）
        private UITextBox _txtHRLow, _txtHRHigh;
        private UITextBox _txtSpO2Low, _txtSpO2High;
        private UITextBox _txtSystolicLow, _txtSystolicHigh;
        private UITextBox _txtDiastolicLow, _txtDiastolicHigh;
        private UITextBox _txtRESPLow, _txtRESPHigh;
        private UITextBox _txtTEMPLow, _txtTEMPHigh;

        private bool IsDesignTime =>
            DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // ---- 无参构造：仅限 VS 设计器 ----
        public SettingsForm()
        {
            InitializeComponent();
        }

        // ---- DI 构造：运行时入口 ----
        public SettingsForm(ThresholdConfigService thresholdConfig) : this()
        {
            _thresholdConfig = thresholdConfig;
            LoadCurrentThresholds();
        }

        private void LoadCurrentThresholds()
        {
            if (_thresholdConfig == null) return;
            SetTxt(_txtHRLow, _thresholdConfig.Defaults["HR"].LowLimit);
            SetTxt(_txtHRHigh, _thresholdConfig.Defaults["HR"].HighLimit);
            SetTxt(_txtSpO2Low, _thresholdConfig.Defaults["SpO2"].LowLimit);
            SetTxt(_txtSpO2High, _thresholdConfig.Defaults["SpO2"].HighLimit);
            SetTxt(_txtSystolicLow, _thresholdConfig.Defaults["NIBP_Systolic"].LowLimit);
            SetTxt(_txtSystolicHigh, _thresholdConfig.Defaults["NIBP_Systolic"].HighLimit);
            SetTxt(_txtDiastolicLow, _thresholdConfig.Defaults["NIBP_Diastolic"].LowLimit);
            SetTxt(_txtDiastolicHigh, _thresholdConfig.Defaults["NIBP_Diastolic"].HighLimit);
            SetTxt(_txtRESPLow, _thresholdConfig.Defaults["RESP"].LowLimit);
            SetTxt(_txtRESPHigh, _thresholdConfig.Defaults["RESP"].HighLimit);
            SetTxt(_txtTEMPLow, _thresholdConfig.Defaults["TEMP"].LowLimit);
            SetTxt(_txtTEMPHigh, _thresholdConfig.Defaults["TEMP"].HighLimit);
        }

        private static void SetTxt(UITextBox txt, double val) => txt.Text = val.ToString("F1");

        private void SaveThresholds()
        {
            _thresholdConfig.Defaults["HR"].LowLimit = Parse(_txtHRLow.Text, 50);
            _thresholdConfig.Defaults["HR"].HighLimit = Parse(_txtHRHigh.Text, 120);
            _thresholdConfig.Defaults["SpO2"].LowLimit = Parse(_txtSpO2Low.Text, 90);
            _thresholdConfig.Defaults["SpO2"].HighLimit = Parse(_txtSpO2High.Text, 100);
            _thresholdConfig.Defaults["NIBP_Systolic"].LowLimit = Parse(_txtSystolicLow.Text, 80);
            _thresholdConfig.Defaults["NIBP_Systolic"].HighLimit = Parse(_txtSystolicHigh.Text, 180);
            _thresholdConfig.Defaults["NIBP_Diastolic"].LowLimit = Parse(_txtDiastolicLow.Text, 50);
            _thresholdConfig.Defaults["NIBP_Diastolic"].HighLimit = Parse(_txtDiastolicHigh.Text, 110);
            _thresholdConfig.Defaults["RESP"].LowLimit = Parse(_txtRESPLow.Text, 8);
            _thresholdConfig.Defaults["RESP"].HighLimit = Parse(_txtRESPHigh.Text, 30);
            _thresholdConfig.Defaults["TEMP"].LowLimit = Parse(_txtTEMPLow.Text, 35.0);
            _thresholdConfig.Defaults["TEMP"].HighLimit = Parse(_txtTEMPHigh.Text, 38.5);
            UIMessageBox.ShowSuccess("阈值配置已保存");
        }

        private static double Parse(string text, double def) =>
            double.TryParse(text, out double r) ? r : def;
    }
}
