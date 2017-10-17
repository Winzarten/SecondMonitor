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
            PluginManager.DeletePlugin(this);
        }

        private void UpdateWaterTemp(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            if (data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature.InCelsius != -1)
                gWaterTemp.Value =(float) data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature.InCelsius;
        }

        private void UpdateFuelLevel(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            gFuel.Value = (float)((data.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters/ data.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity.InLiters) * 100);
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
            if(data.PlayerInfo == null)
                return;
            gMeter1.VertG = -data.PlayerInfo.CarInfo.Acceleration.ZInG;
            gMeter1.HorizG = data.PlayerInfo.CarInfo.Acceleration.XInG;
            gMeter1.Refresh();
            if (data.PlayerInfo.CarInfo.FuelSystemInfo.FuelPressure.InKpa > 10)
                blbFuelPressure.Color = Color.Green;
            else
                blbFuelPressure.Color = Color.Red;
            label1.Text = data.SessionInfo.SessionTime.TotalSeconds.ToString();
        }


        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            SimulatorDataSet data = args.Data;
            if (data.PlayerInfo == null || args.Data.PlayerInfo.CarInfo.WaterSystmeInfo.WaterTemperature.InCelsius == -1)
                return;
            UpdateGui(data);
        }

        private void stayOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }
    }
}
