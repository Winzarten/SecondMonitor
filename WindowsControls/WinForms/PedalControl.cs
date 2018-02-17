namespace SecondMonitor.WindowsControls.WinForms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Snapshot;

    public partial class PedalControl : UserControl
    {
        public PedalControl()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            PerformAutoScale();
            UpdatePostions();
            

        }

        private void UpdatePostions()
        {
            UpdateControlLocation(pnlThrottle, 2);
            UpdateControlLocation(lblThrottle, 2);

            UpdateControlLocation(pnlBrake, 1);
            UpdateControlLocation(lblBrake, 1);

            UpdateControlLocation(pnlClutch, 0);
            UpdateControlLocation(lblClutch, 0);
        }

        private void UpdateControlLocation(Control control, int postion)
        {
            Point loc = control.Location;
            loc.X = (Width / 3) * postion;
            control.Location = loc;
            control.Width = Width / 3;
        }

        public void UpdateControl(SimulatorDataSet data)
        {
            int refHeight = Height - 30;
            UpdatePostions();
            UpdateControlsByValue(pnlThrottle, lblThrottle, data.PedalInfo.ThrottlePedalPosition);
            UpdateControlsByValue(pnlBrake, lblBrake, data.PedalInfo.BrakePedalPosition);
            UpdateControlsByValue(pnlClutch, lblClutch, data.PedalInfo.ClutchPedalPosition);
        }

        private void UpdateControlsByValue(Panel panel, Label label, double  value)
        {
            int refHeight = Height - 30;

            Point loc = panel.Location;
            loc.Y = 0;
            panel.Height = (int)(refHeight * value);
            panel.Location = loc;
            label.Text = (value * 100).ToString("0.0");
        }

        private void lblThrottle_Click(object sender, EventArgs e)
        {

        }
    }
    
}
