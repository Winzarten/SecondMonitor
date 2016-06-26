using System;
using System.Collections.Generic;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using System.Windows.Forms;

namespace SecondMonitor.PluginManager.Core
{
    public class PluginsManager
    {
        public event EventHandler<DataEventArgs> DataLoaded;
        private ICollection<ISecondMonitorPlugin> plugins;

        public PluginsManager(IGameConnector connector)
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

        public IGameConnector Connector
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
