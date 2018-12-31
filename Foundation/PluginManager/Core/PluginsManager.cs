namespace SecondMonitor.PluginManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using NLog;
    using NLog.Fluent;
    using DataModel.Snapshot;
    using GameConnector;

    public class PluginsManager
    {
        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<MessageArgs> DisplayMessage;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ISecondMonitorPlugin> _plugins;

        private IGameConnector _activeConnector;
        private Task _connectorTask;
        private SimulatorDataSet _oldDataSet;

        public PluginsManager(IGameConnector[] connectors)
        {
            Logger.Info().TimeStamp(DateTime.Now).Message("Fooo");
            _plugins = new List<ISecondMonitorPlugin>();
            Connectors = connectors;
        }

        private static void LogSimulatorDataSet(SimulatorDataSet dataSet)
        {
            // Logger.Info("Simulator set: {0}", DataModelSerializerHelper.ToJson(dataSet));
        }

        private void Connector_Disconnected(object sender, EventArgs e)
        {
            if (_activeConnector != sender)
            {
                return;
            }

            Logger.Info("Connector Disconnected: " + _activeConnector.GetType());
            _activeConnector.DataLoaded -= OnDataLoaded;
            _activeConnector.SessionStarted -= OnSessionStarted;
            _activeConnector.Disconnected -= Connector_Disconnected;
            _activeConnector.DisplayMessage -= ActiveConnectorOnDisplayMessage;
            _activeConnector = null;
            RaiseSessionStartedEvent(new SimulatorDataSet("Not Connected"));
        }

        public void Start()
        {
            _connectorTask = ConnectorDaemonMethod();
            Logger.Info("-----------------------Application Started------------------------------------");
        }

        private async Task ConnectorDaemonMethod()
        {
            while (true)
            {
                if (_activeConnector == null)
                {
                    await ConnectLoop();
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        private async Task ConnectLoop()
        {
            while (true)
            {
                await Task.Delay(5000).ConfigureAwait(false);
                foreach (var connector in Connectors)
                {
                    connector.DisplayMessage += ActiveConnectorOnDisplayMessage;
                    if (connector.TryConnect())
                    {
                        Logger.Info("Connector Connected: " + connector.GetType());
                        _activeConnector = connector;
                        _activeConnector.DataLoaded += OnDataLoaded;
                        _activeConnector.SessionStarted += OnSessionStarted;
                        _activeConnector.Disconnected += Connector_Disconnected;
                        return;
                    }

                    connector.DisplayMessage -= ActiveConnectorOnDisplayMessage;
                }
            }
        }

        private void ActiveConnectorOnDisplayMessage(object sender, MessageArgs messageArgs)
        {
            DisplayMessage?.Invoke(this, messageArgs);
        }

        public void InitializePlugins()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            IEnumerable<string> files = Directory.EnumerateFiles(pluginsDirectory, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string assemblyPath = Path.Combine(pluginsDirectory, file);
                _plugins.AddRange(GetPluginsFromAssembly(assemblyPath));
            }

            if (_plugins.Count == 0)
            {
                MessageBox.Show("No plugins loaded. Please place plugins .dll into " + pluginsDirectory, "No plugins", MessageBoxButtons.OK);
                Environment.Exit(1);
            }

            foreach (ISecondMonitorPlugin plugin in _plugins)
            {
                Logger.Info("Running plugin " + plugin.GetType());
                plugin.PluginManager = this;
                plugin.RunPlugin();
            }
        }

        public ICollection<ISecondMonitorPlugin> GetPluginsFromAssembly(string assemblyPath)
        {
            var secondMonitorPluginType = typeof(ISecondMonitorPlugin);
            var plugins = new List<ISecondMonitorPlugin>();
            Assembly assembly;
            try
            {
                assembly = Assembly.UnsafeLoadFrom(assemblyPath);
                Logger.Info("Searching Assembly: " + assemblyPath);


                var types = assembly.GetTypes().Where(c => !c.IsInterface && secondMonitorPluginType.IsAssignableFrom(c));
                foreach (Type type in types)
                {
                    ISecondMonitorPlugin plugin = Activator.CreateInstance(type) as ISecondMonitorPlugin;
                    plugins.Add(plugin);
                    Logger.Info("Found plugin:" + type);
                }

                return plugins;
            }
            catch (Exception)
            {
                return new List<ISecondMonitorPlugin>();
            }
        }

        public void DeletePlugin(ISecondMonitorPlugin plugin, List<Exception> experiencedExceptions)
        {
            lock (_plugins)
            {
                Logger.Info("Plugin " + plugin.GetType() + " closed");
                _plugins.Remove(plugin);

                experiencedExceptions.ForEach(e => Logger.Error(e));

                bool allDaemons = _plugins.Aggregate(true, (current, activePlugin) => current && activePlugin.IsDaemon);
                if (!allDaemons)
                {
                    return;
                }

                Logger.Info("------------------------------All plugins closed - application exiting-------------------------------\n\n\n");
                Application.Exit(new CancelEventArgs(true));
            }
        }

        public void DeletePlugin(ISecondMonitorPlugin plugin)
        {
            DeletePlugin(plugin, new List<Exception>());
        }

        public IGameConnector[] Connectors { get; }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            RaiseDataLoadedEvent(args.Data);
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {
            RaiseSessionStartedEvent(args.Data);
        }

        private void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            Logger.Info("New Session starting");
            if (_oldDataSet != null)
            {
                Logger.Info("Old set:");
                LogSimulatorDataSet(_oldDataSet);
            }

            _oldDataSet = data;
            Logger.Info("New set:");
            LogSimulatorDataSet(_oldDataSet);
            SessionStarted?.Invoke(this, args);
        }

        private void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            try
            {
                DataEventArgs args = new DataEventArgs(data);
                _oldDataSet = data;
                DataLoaded?.Invoke(this, args);
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
            }
            catch (Exception)
            {
                LogSimulatorDataSet(_oldDataSet);
                throw;
            }
        }
    }
}