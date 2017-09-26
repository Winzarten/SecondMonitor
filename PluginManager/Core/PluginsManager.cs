using System;
using System.Collections.Generic;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SecondMonitor.PluginManager.Core
{
    public class PluginsManager
    {
        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<DataEventArgs> SessionStarted;
        private List<ISecondMonitorPlugin> plugins;

        public PluginsManager(IGameConnector connector)
        {
            plugins = new List<ISecondMonitorPlugin>();
            Connector = connector;
            connector.DataLoaded += OnDataLoaded;
            connector.SessionStarted += OnSessionStarted;
        }

        public void InitializePlugins()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            IEnumerable<string> files = Directory.EnumerateFiles(pluginsDirectory, "*.dll",SearchOption.AllDirectories);
            foreach(String file in files)
            {
                string assemblyPath = Path.Combine(pluginsDirectory, file);                
                plugins.AddRange(GetPluginsFromAssembly(assemblyPath));
                
            }
            if(plugins.Count == 0)
            {
                MessageBox.Show("No plugins loaded. Please place plugins .dll into " + pluginsDirectory, "No plugins", MessageBoxButtons.OK);
                System.Environment.Exit(1);
            }
            foreach(ISecondMonitorPlugin plugin in plugins)
            {                
                plugin.PluginManager = this;
                plugin.RunPlugin();                
            }
            
            /*plugins = new List<ISecondMonitorPlugin>();
            ISecondMonitorPlugin plugin = new CarStatusForm();
            plugin.PluginManager = this;
            plugin.RunPlugin();
            plugins.Add(plugin);*/
        }

        public ICollection<ISecondMonitorPlugin> GetPluginsFromAssembly(string assemblyPath)
        {
            var secondMonitorPluginType = typeof(ISecondMonitorPlugin);
            List<ISecondMonitorPlugin> plugins = new List<ISecondMonitorPlugin>();
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(assemblyPath);
                
            }catch(Exception)
            {
                return new List<ISecondMonitorPlugin>();
            }
            IEnumerable<Type> types = assembly.GetTypes().Where(c => !(c.IsInterface) && secondMonitorPluginType.IsAssignableFrom(c));
            foreach(Type type in types)
            {
                ISecondMonitorPlugin plugin = Activator.CreateInstance(type) as ISecondMonitorPlugin;
                plugins.Add(plugin);
            }
            return plugins;
            
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

        void OnSessionStarted(object sender, DataEventArgs args)
        {
            RaiseSessionStartedEvent(args.Data);
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            if (handler != null)
            {
                handler(this, args);
            }
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
