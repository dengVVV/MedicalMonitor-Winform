using MedicalMonitor.WinForm.Models;
using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    public partial class BedCardControl : UIUserControl
    {
        private UILabel _lblBedNo, _lblPatientName, _lblPatientAge, _lblHR, _lblSpO2, _lblNIBP, _lblStatus;
        private UILight _statusLight;
        private BedMappingModel _binding;

        public event EventHandler<BedMappingModel> BedSelected;

        public BedMappingModel Binding
        {
            get => _binding;
            set { _binding = value; RefreshDisplay(); }
        }

        public BedCardControl()
        {
            InitializeComponent();
            this.Click += OnCardClick;
            foreach (Control c in this.Controls) c.Click += OnCardClick;
        }

        public void RefreshDisplay()
        {
            if (_binding == null) return;
            _lblBedNo.Text = _binding.BedNo;
            _lblPatientName.Text = (_binding.IsOccupied && _binding.Patient != null) ? _binding.Patient.Name : "(空床)";
            _lblPatientAge.Text = (_binding.IsOccupied && _binding.Patient != null) ? $"{_binding.Patient.Age}岁  {(_binding.Patient.Gender == Models.Enums.Gender.Male ? "男" : "女")}" : "";
            var v = _binding.LatestVitalSigns;
            if (v != null && v.IsValid)
            {
                _lblHR.Text = $"HR: {v.HeartRate?.ToString("F0") ?? "--"}";
                _lblSpO2.Text = $"SpO2: {v.SpO2?.ToString("F0") ?? "--"}%";
                _lblNIBP.Text = $"NIBP: {v.NIBP_Systolic?.ToString("F0") ?? "--"} / " +
                    $"{v.NIBP_Diastolic?.ToString("F0") ?? "--"}";
            }
            else { _lblHR.Text = "HR: --"; _lblSpO2.Text = "SpO2: --%"; _lblNIBP.Text = "NIBP: --/--"; }
            _statusLight.State = _binding.IsOnline ? UILightState.On : UILightState.Off;
            _statusLight.OnColor = Color.LimeGreen;
            _lblStatus.Text = _binding.IsOnline ? "在线" : "离线";
        }

        private void OnCardClick(object sender, EventArgs e) => BedSelected?.Invoke(this, _binding);
    }
}
