using SecondWindow.Core.PluginManager;
using SecondWindow.Core.R3EConnector;
using SecondWindow.Core.R3EConnector.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondWindow.CarStatus.Forms
{
    public partial class CarStatusForm : Form, ISecondWindowPlugin
    {

        delegate void UpdateGuiDelegate(R3ESharedData data);

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

        private PluginManager pluginManager;
        public PluginManager PluginManager
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

        private void UpdateWaterTemp(R3ESharedData data)
        {
            if (data.EngineWaterTemp != -1)
                gWaterTemp.Value = data.EngineWaterTemp;
        }

        private void UpdateFuelLevel(R3ESharedData data)
        {
            gFuel.Value = (data.FuelLeft/ data.FuelCapacity) * 100;
        }
                   
        private void UpdateGui(R3ESharedData data)
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
        }


        private void OnDataLoaded(object sender, R3EDataEventArgs args)
        {
            R3ESharedData data = args.Data;
            if (args.Data.EngineWaterTemp == -1)
                return;
            UpdateGui(data);
        }
    }
}
