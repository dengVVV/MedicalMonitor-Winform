using OxyPlot.WindowsForms;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    partial class WaveformViewControl
    {
        private void InitializeComponent()
        {
            this._plotView = new PlotView();

            this.SuspendLayout();

            // WaveformViewControl
            this.Size = new System.Drawing.Size(600, 300);
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 40);

            // _plotView
            this._plotView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._plotView.BackColor = System.Drawing.Color.FromArgb(30, 30, 40);

            this.Controls.Add(this._plotView);

            this.ResumeLayout(false);

            // PlotModel / Axes / Series 不是 WinForms 控件，设计器不解析，移到运行时方法
            this.InitializePlotModel();
        }
    }
}