using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondMonitor.Core.R3EConnector;
using SecondMonitor.Core.R3EConnector.Data;
using SecondMonitor.DataModel;

namespace SecondMonitor.CarStatus.Forms.Controls
{
    public partial class PedalControl : UserControl
    {
        public PedalControl()
        {
            InitializeComponent();
            UpdatePostions();
            

        }

        private void UpdatePostions()
        {
            UpdateControlLocation(pnlThrottle, 2);
            UpdateControlLocation(lblThrottle, 2);

            UpdateControlLocation(pnlBrake, 1);
            UpdateControlLocation(lblBrake, 1);
        }

        private void UpdateControlLocation(Control control,int postion)
        {
            Point loc = control.Location;
            loc.X = (this.Width / 3) * postion;
            control.Location = loc;
            control.Width = this.Width / 3;
        }

        public void UpdateControl(SimulatorDataSet data)
        {
            int refHeight = this.Height - 30;
            UpdatePostions();
            UpdateControlsByValue(pnlThrottle, lblThrottle, data.PedalInfo.ThrottlePedalPosition);
            UpdateControlsByValue(pnlBrake, lblBrake, data.PedalInfo.BrakePedalPosition);
        }

        private void UpdateControlsByValue(Panel panel, Label label, double  value)
        {
            int refHeight = this.Height - 30;

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
