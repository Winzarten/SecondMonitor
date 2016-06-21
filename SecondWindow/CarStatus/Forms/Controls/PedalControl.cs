using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondWindow.Core.R3EConnector;
using SecondWindow.Core.R3EConnector.Data;

namespace SecondWindow.CarStatus.Forms.Controls
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

        public void UpdateControl(R3ESharedData data)
        {
            int refHeight = this.Height - 30;

            UpdateControlsByValue(pnlThrottle, lblThrottle, data.ThrottlePedal);
            UpdateControlsByValue(pnlBrake, lblBrake, data.BrakePedal);
        }

        private void UpdateControlsByValue(Panel panel, Label label, float value)
        {
            int refHeight = this.Height - 30;

            Point loc = panel.Location;
            loc.Y = (int)(refHeight * (1 - value));
            panel.Height = (refHeight - panel.Location.Y);
            panel.Location = loc;
            label.Text = (value * 100).ToString("0.0");
        }

        private void lblThrottle_Click(object sender, EventArgs e)
        {

        }
    }
    
}
