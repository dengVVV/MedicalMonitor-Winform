using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.Controls
{
    partial class VitalSignPanelControl
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(220, 380);
            this.FillColor = Color.FromArgb(245, 248, 252);
            this.Radius = 8;
            int y = 15, h = 68;
            this._rowHR = new VitalSignRow("HR", "bpm", Color.FromArgb(220, 50, 50)) { Location = new Point(10, y) };
            this._rowSpO2 = new VitalSignRow("SpO2", "%", Color.FromArgb(0, 120, 200)) { Location = new Point(10, y + h) };
            this._rowNIBP = new VitalSignRow("NIBP", "mmHg", Color.FromArgb(80, 80, 200)) { Location = new Point(10, y + h * 2) };
            this._rowRESP = new VitalSignRow("RESP", "次/分", Color.FromArgb(0, 150, 100)) { Location = new Point(10, y + h * 3) };
            this._rowTEMP = new VitalSignRow("TEMP", "℃", Color.FromArgb(200, 150, 0)) { Location = new Point(10, y + h * 4) };
            this.Controls.Add(this._rowHR); this.Controls.Add(this._rowSpO2); this.Controls.Add(this._rowNIBP);
            this.Controls.Add(this._rowRESP); this.Controls.Add(this._rowTEMP);
            this.ResumeLayout(false);
        }
    }
}
