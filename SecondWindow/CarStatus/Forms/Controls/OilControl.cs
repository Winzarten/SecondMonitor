using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondWindow.Core.R3EConnector.Data;

namespace SecondWindow.CarStatus.Forms.Controls
{
    public partial class OilControl : UserControl
    {
        public OilControl()
        {
            InitializeComponent();
        }

        private void gPressure_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        public void UpdateControl(R3ESharedData data)
        {
            gTemperature.Value = data.EngineOilTemp;
            gPressure.Value = data.EngineOilPressure;
        }
    }
}
