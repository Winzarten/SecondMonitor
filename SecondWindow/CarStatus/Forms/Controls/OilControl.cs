using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondMonitor.Core.R3EConnector.Data;
using SecondMonitor.DataModel;

namespace SecondMonitor.CarStatus.Forms.Controls
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

        public void UpdateControl(SimulatorDataSet data)
        {
            gTemperature.Value = (float) data.PlayerCarInfo.OilSystemInfo.OilTemperature.InCelsius;
            gPressure.Value = (float) data.PlayerCarInfo.OilSystemInfo.OilPressure.InKpa;
        }
    }
}
