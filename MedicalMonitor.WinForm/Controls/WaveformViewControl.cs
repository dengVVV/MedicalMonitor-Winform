using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    public partial class WaveformViewControl : UIUserControl
    {
        private PlotView _plotView;
        private PlotModel _plotModel;
        private LineSeries _hrSeries, _spo2Series;
        private readonly Queue<DataPoint> _hrBuf = new Queue<DataPoint>();
        private readonly Queue<DataPoint> _spo2Buf = new Queue<DataPoint>();
        private double _time;
        private const double WinSec = 60;

        public bool ShowSpO2 { get; set; } = true;

        public WaveformViewControl()
        {
            InitializeComponent();
        }

        public void AddDataPoint(double? hr, double? spo2)
        {
            _time++;
            if (hr.HasValue) { _hrBuf.Enqueue(new DataPoint(_time, hr.Value)); if (_hrBuf.Count > WinSec * 2) _hrBuf.Dequeue(); }
            if (spo2.HasValue) { _spo2Buf.Enqueue(new DataPoint(_time, spo2.Value)); if (_spo2Buf.Count > WinSec * 2) _spo2Buf.Dequeue(); }
            UpdateSeries();
        }

        public void InitializeWithHistory(IEnumerable<(double Time, double? Hr, double? SpO2)> h)
        {
            _hrBuf.Clear(); _spo2Buf.Clear(); _time = 0;
            foreach (var (t, hr, spo2) in h) { _time = t; if (hr.HasValue) _hrBuf.Enqueue(new DataPoint(t, hr.Value)); if (spo2.HasValue) _spo2Buf.Enqueue(new DataPoint(t, spo2.Value)); }
            UpdateSeries();
        }

        private void UpdateSeries()
        {
            _hrSeries.Points.Clear(); _spo2Series.Points.Clear();
            foreach (var p in _hrBuf) _hrSeries.Points.Add(p);
            foreach (var p in _spo2Buf) _spo2Series.Points.Add(p);
            double xMin = Math.Max(0, _time - WinSec), xMax = Math.Max(WinSec, _time);
            _plotModel.Axes[0].Minimum = xMin; _plotModel.Axes[0].Maximum = xMax;
            _plotModel.InvalidatePlot(true); _plotView.InvalidatePlot(true);
        }

        private void InitializePlotModel()
        {
            this._plotModel = new PlotModel
            {
                Title = null,
                PlotAreaBorderThickness = new OxyThickness(0),
                Background = OxyColor.FromRgb(30, 30, 40),
                PlotAreaBackground = OxyColor.FromRgb(30, 30, 40)
            };

            this._plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = WinSec,
                IsAxisVisible = false,
                IsZoomEnabled = false
            });

            this._plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 30,
                Maximum = 180,
                Title = "HR (bpm)",
                TitleColor = OxyColor.FromRgb(255, 80, 80),
                TextColor = OxyColor.FromRgb(200, 200, 200),
                AxislineColor = OxyColor.FromRgb(60, 60, 60),
                TicklineColor = OxyColor.FromRgb(60, 60, 60),
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(50, 50, 60),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(40, 40, 50)
            });

            this._plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Minimum = 70,
                Maximum = 100,
                Title = "SpO2 (%)",
                TitleColor = OxyColor.FromRgb(80, 200, 255),
                TextColor = OxyColor.FromRgb(200, 200, 200),
                AxislineColor = OxyColor.FromRgb(60, 60, 60),
                TicklineColor = OxyColor.FromRgb(60, 60, 60),
                Key = "SpO2"
            });

            this._hrSeries = new LineSeries
            {
                Title = "HR",
                Color = OxyColor.FromRgb(255, 80, 80),
                StrokeThickness = 2.0,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None,
                YAxisKey = null
            };

            this._spo2Series = new LineSeries
            {
                Title = "SpO2",
                Color = OxyColor.FromRgb(80, 200, 255),
                StrokeThickness = 1.5,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None,
                YAxisKey = "SpO2"
            };

            this._plotModel.Series.Add(this._hrSeries);
            this._plotModel.Series.Add(this._spo2Series);

            this._plotView.Model = this._plotModel;
        }


    }
}
