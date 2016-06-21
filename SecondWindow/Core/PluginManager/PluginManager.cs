using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecondWindow.Core.R3EConnector;
using SecondWindow.Core.R3EConnector.Data;
using SecondWindow.CarStatus.Forms;
using System.Windows.Forms;

namespace SecondWindow.Core.PluginManager
{
    public class PluginManager
    {
        public event EventHandler<R3EDataEventArgs> DataLoaded;
        private ICollection<ISecondWindowPlugin> plugins;

        public PluginManager(IR3EConnector connector)
        {
            plugins = new List<ISecondWindowPlugin>();
            Connector = connector;
            connector.DataLoaded += OnDataLoaded;
        }

        public void InitializePlugins()
        {
            plugins = new List<ISecondWindowPlugin>();
            ISecondWindowPlugin plugin = new CarStatusForm();
            plugin.PluginManager = this;
            plugin.RunPlugin();
            plugins.Add(plugin);
        }

        public void DeletePlugin(ISecondWindowPlugin plugin)
        {   lock (plugins)
            {
                plugins.Remove(plugin);
                bool allDaemons = true;
                foreach(ISecondWindowPlugin activePlugin in plugins)
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

        void OnDataLoaded(object sender, R3EDataEventArgs args)
        {
            RaiseDataLoadedEvent(args.Data);
        }

        private void RaiseDataLoadedEvent(R3ESharedData data)
        {
            R3EDataEventArgs args = new R3EDataEventArgs(data);
            EventHandler<R3EDataEventArgs> handler = DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

    }
}
