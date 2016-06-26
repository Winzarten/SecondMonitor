using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecondMonitor.Core.R3EConnector;
using SecondMonitor.Core.R3EConnector.Data;
using SecondMonitor.CarStatus.Forms;
using System.Windows.Forms;
using SecondMonitor.DataModel;

namespace SecondMonitor.Core.PluginManager
{
    public class PluginManager
    {
        public event EventHandler<DataEventArgs> DataLoaded;
        private ICollection<ISecondMonitorPlugin> plugins;

        public PluginManager(IR3EConnector connector)
        {
            plugins = new List<ISecondMonitorPlugin>();
            Connector = connector;
            connector.DataLoaded += OnDataLoaded;
        }

        public void InitializePlugins()
        {
            plugins = new List<ISecondMonitorPlugin>();
            ISecondMonitorPlugin plugin = new CarStatusForm();
            plugin.PluginManager = this;
            plugin.RunPlugin();
            plugins.Add(plugin);
        }

        public void DeletePlugin(ISecondMonitorPlugin plugin)
        {   lock (plugins)
            {
                plugins.Remove(plugin);
                bool allDaemons = true;
                foreach(ISecondMonitorPlugin activePlugin in plugins)
                {
                    allDaemons = allDaemons && activePlugin.IsDaemon;
                }
                if (allDaemons)
                    Application.Exit();
                
             }
        }

        public IR3EConnector Connector
        {
            get;
            private set;
        }

        void OnDataLoaded(object sender, DataEventArgs args)
        {
            RaiseDataLoadedEvent(args.Data);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

    }
}
