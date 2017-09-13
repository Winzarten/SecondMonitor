using System;
using System.Windows.Forms;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.Core;
using SecondMonitor.PluginManager.GameConnector;
using System.Drawing;

namespace SecondMonitor.CarStatus.Forms
{
    public partial class CarStatusForm : Form, ISecondMonitorPlugin
    {

        delegate void UpdateGuiDelegate(SimulatorDataSet data);

        public CarStatusForm()
        {
            InitializeComponent();
        }

        public bool IsDaemon
        {
            get
            {
                return false;
            }
        }

        private PluginsManager pluginManager;
        public PluginsManager PluginManager
        {
            get
            {
                return pluginManager;
            }
            set
            {
                pluginManager = value;
                pluginManager.DataLoaded += OnDataLoaded;
            }
        }

        public void RunPlugin()
        {
            this.Show();
        }

        private void CarStatusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PluginManager.DeletePlugin(this) ;
        }

        private void UpdateWaterTemp(SimulatorDataSet data)
        {
            if (data.PlayerCarInfo.WaterSystmeInfo.WaterTemperature.InCelsius != -1)
                gWaterTemp.Value =(float) data.PlayerCarInfo.WaterSystmeInfo.WaterTemperature.InCelsius;
        }

        private void UpdateFuelLevel(SimulatorDataSet data)
        {
            gFuel.Value = (float)((data.PlayerCarInfo.FuelSystemInfo.FuelRemaining.InLiters/ data.PlayerCarInfo.FuelSystemInfo.FuelCapacity.InLiters) * 100);
        }
                   
        private void UpdateGui(SimulatorDataSet data)
        {            
            if (Disposing || IsDisposed)
                return;
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new UpdateGuiDelegate(UpdateGui), data);
                    return;
                }catch(Exception)
                {
                    return;
                }
            }
            UpdateWaterTemp(data);
            UpdateFuelLevel(data);
            pedalControl1.UpdateControl(data);
            oilControl2.UpdateControl(data);
            ctlRearLeft.UpdateControl(data);
            ctlRearRight.UpdateControl(data);
            ctlFrontLeft.UpdateControl(data);
            ctlFrontRight.UpdateControl(data);
            gMeter1.VertG = -data.PlayerCarInfo.Acceleration.ZInG;
            gMeter1.HorizG = data.PlayerCarInfo.Acceleration.XInG;
            gMeter1.Refresh();
            if (data.PlayerCarInfo.FuelSystemInfo.FuelPressure.InKpa > 10)
                blbFuelPressure.Color = Color.Green;
            else
                blbFuelPressure.Color = Color.Red;
            label1.Text = data.SessionInfo.SessionTime.TotalSeconds.ToString();
        }


        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            SimulatorDataSet data = args.Data;
            if (args.Data.PlayerCarInfo.WaterSystmeInfo.WaterTemperature.InCelsius == -1)
                return;
            UpdateGui(data);
        }

        private void stayOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }
    }
}
