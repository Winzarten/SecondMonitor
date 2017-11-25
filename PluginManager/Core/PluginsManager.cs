using System;
using System.Collections.Generic;
using SecondMonitor.DataModel;
using SecondMonitor.PluginManager.GameConnector;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NLog;
using NLog.Fluent;

namespace SecondMonitor.PluginManager.Core
{
    public class PluginsManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<DataEventArgs> SessionStarted;
        private List<ISecondMonitorPlugin> _plugins;
        private IGameConnector _activeConnector;
        private Thread _connectorDaemon;
        private SimulatorDataSet _olDataSet;

        public PluginsManager(IGameConnector[] connectors)
        {
            _logger.Info().TimeStamp(DateTime.Now).Message("Fooo");
            _plugins = new List<ISecondMonitorPlugin>();
            Connectors = connectors;
        }

        private void Connector_Disconnected(object sender, EventArgs e)
        {
            if (_activeConnector == sender)
            {
                _logger.Info("Connector Disconnected: " + _activeConnector.GetType());
                _activeConnector.DataLoaded -= OnDataLoaded;
                _activeConnector.SessionStarted -= OnSessionStarted;
                _activeConnector.Disconnected -= Connector_Disconnected;
                _activeConnector = null;
                RaiseSessionStartedEvent(new SimulatorDataSet("Not Connected"));
            }
        }

        public void Start()
        {
            _connectorDaemon = new Thread(ConnectorDaemonMethod);
            _connectorDaemon.IsBackground = true;
            _connectorDaemon.Start();
            _logger.Info("-----------------------Application Started------------------------------------");
        }

        private void ConnectorDaemonMethod()
        {
            while(true)
            {
                if(_activeConnector == null)
                    ConnectLoop();
                Thread.Sleep(1000);
            }
        }

        private void ConnectLoop()
        {
            while (true)
            {
                Thread.Sleep(100);
                foreach (var connector in Connectors)
                {
                    if (connector.TryConnect())
                    {
                        _logger.Info("Connector Connected: "+connector.GetType());
                        _activeConnector = connector;
                        _activeConnector.DataLoaded += OnDataLoaded;
                        _activeConnector.SessionStarted += OnSessionStarted;
                        _activeConnector.Disconnected += Connector_Disconnected;
                        return;
                    }
                }
            }
        }

        public void InitializePlugins()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            IEnumerable<string> files = Directory.EnumerateFiles(pluginsDirectory, "*.dll",SearchOption.AllDirectories);
            foreach(String file in files)
            {
                string assemblyPath = Path.Combine(pluginsDirectory, file);
                _plugins.AddRange(GetPluginsFromAssembly(assemblyPath));
                
            }
            if(_plugins.Count == 0)
            {
                MessageBox.Show("No plugins loaded. Please place plugins .dll into " + pluginsDirectory, "No plugins", MessageBoxButtons.OK);
                System.Environment.Exit(1);
            }
            foreach(ISecondMonitorPlugin plugin in _plugins)
            {
                _logger.Info("Running plugin " + plugin.GetType());
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
                assembly = Assembly.UnsafeLoadFrom(assemblyPath);
                _logger.Info("Searching Assembly: " + assemblyPath);

            }
            catch(Exception)
            {
                return new List<ISecondMonitorPlugin>();
            }
            IEnumerable<Type> types = assembly.GetTypes().Where(c => !(c.IsInterface) && secondMonitorPluginType.IsAssignableFrom(c));
            foreach(Type type in types)
            {
                ISecondMonitorPlugin plugin = Activator.CreateInstance(type) as ISecondMonitorPlugin;
                plugins.Add(plugin);
                _logger.Info("Found plugin:" + type.ToString());
                
            }
            return plugins;
            
        }

        

        public void DeletePlugin(ISecondMonitorPlugin plugin)
        {   lock (_plugins)
            {
                _logger.Info("Plugin " + plugin.GetType() + " closed");
                _plugins.Remove(plugin);
                bool allDaemons = true;
                foreach(ISecondMonitorPlugin activePlugin in _plugins)
                {
                    allDaemons = allDaemons && activePlugin.IsDaemon;
                }
                if (allDaemons)
                {
                    _logger.Info("------------------------------All plugins closed - application exiting-------------------------------\n\n\n");
                    Application.Exit();
                }

            }
        }

        public IGameConnector[] Connectors
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
            _logger.Info("New Session starting");
            if (_olDataSet != null)
            {
                _logger.Info("Old set:");
                LogSimulatorDataSet(_olDataSet);
            }
            _olDataSet = data;
            _logger.Info("New set:");
            LogSimulatorDataSet(_olDataSet);
            SessionStarted?.Invoke(this, args);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            _olDataSet = data;
            DataLoaded?.Invoke(this, args);
        }

        private void LogSimulatorDataSet(SimulatorDataSet dataSet)
        {
            _logger.Info("Simulator set: {0}", DatamodeSerializerHelper.ToJson(dataSet));
        }

    }
}
