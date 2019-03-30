﻿namespace SecondMonitor.PluginManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using NLog;
    using DataModel.Snapshot;
    using GameConnector;
    using PluginsConfiguration.Common.Controller;

    public class PluginsManager
    {
        private readonly IPluginSettingsProvider _pluginSettingsProvider;
        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<MessageArgs> DisplayMessage;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ISecondMonitorPlugin> _plugins;

        private IGameConnector _activeConnector;
        private Task _connectorTask;
        private SimulatorDataSet _oldDataSet;
        private readonly List<ISimulatorDateSetVisitor> _visitors;

        public PluginsManager(IPluginSettingsProvider pluginSettingsProvider, IGameConnector[] connectors, IEnumerable<ISimulatorDateSetVisitor> dataVisitors)
        {
            _pluginSettingsProvider = pluginSettingsProvider;
            _plugins = new List<ISecondMonitorPlugin>();
            Connectors = connectors;
            _visitors = dataVisitors.ToList();
        }

        private static void LogSimulatorDataSet(SimulatorDataSet dataSet)
        {
            // Logger.Info("Simulator set: {0}", DataModelSerializerHelper.ToJson(dataSet));
        }

        private async void Connector_Disconnected(object sender, EventArgs e)
        {
            if (_activeConnector != sender)
            {
                return;
            }

            await _activeConnector.FinnishConnectorAsync();

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
                        _activeConnector.StartConnectorLoop();
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

        public ICollection<ISecondMonitorPlugin> GetPluginsFromPath(string mainDirectory, bool includeDisabled)
        {
            List<ISecondMonitorPlugin> plugins = new List<ISecondMonitorPlugin>();
            IEnumerable<string> files = Directory.EnumerateFiles(mainDirectory, "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string assemblyPath = Path.Combine(mainDirectory, file);
                plugins.AddRange(GetPluginsFromAssembly(assemblyPath, includeDisabled));
            }

            return plugins;
        }

        public void InitializePlugins()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            _plugins.AddRange(GetPluginsFromPath(pluginsDirectory, false));

            if (_plugins.Count == 0)
            {
                MessageBox.Show("No enabled plugins loaded. Please enabled some of existing plugins, or place plugins .dll into " + pluginsDirectory, "No plugins", MessageBoxButtons.OK);
                Environment.Exit(1);
            }

            foreach (ISecondMonitorPlugin plugin in _plugins)
            {
                Logger.Info("Running plugin " + plugin.GetType());
                plugin.PluginManager = this;
                plugin.RunPlugin();
            }
        }

        public ICollection<ISecondMonitorPlugin> GetPluginsFromAssembly(string assemblyPath, bool includeDisabled)
        {
            Type secondMonitorPluginType = typeof(ISecondMonitorPlugin);
            List<ISecondMonitorPlugin> plugins = new List<ISecondMonitorPlugin>();
            try
            {
                Assembly assembly = Assembly.UnsafeLoadFrom(assemblyPath);
                Logger.Info("Searching Assembly: " + assemblyPath);


                var types = assembly.GetTypes().Where(c => !c.IsInterface && secondMonitorPluginType.IsAssignableFrom(c));
                foreach (Type type in types)
                {
                    ISecondMonitorPlugin plugin = Activator.CreateInstance(type) as ISecondMonitorPlugin;
                    if (!includeDisabled && !IsPluginEnabled(plugin))
                    {
                        continue;
                    }
                    plugins.Add(plugin);
                    Logger.Info("Found plugin:" + type);
                }

                return plugins;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting plugins from assembly");
                return new List<ISecondMonitorPlugin>();
            }
        }

        public async Task DeletePlugin(ISecondMonitorPlugin plugin, List<Exception> experiencedExceptions)
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
            }

            if (_activeConnector != null)
            {
                await _activeConnector.FinnishConnectorAsync();
            }

            Logger.Info("------------------------------All plugins closed - application exiting-------------------------------\n\n\n");
            Application.Exit(new CancelEventArgs(true));
        }

        public IGameConnector[] Connectors { get; }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            _visitors.ForEach(x =>  args.Data.Accept(x));
            RaiseDataLoadedEvent(args.Data);
        }

        private bool IsPluginEnabled(ISecondMonitorPlugin plugin)
        {
            if (_pluginSettingsProvider.TryIsPluginEnabled(plugin.PluginName, out bool isEnabled))
            {
                Logger.Info($"Plugin {plugin.PluginName} is Enabled: {isEnabled}");
                return isEnabled;
            }
            _pluginSettingsProvider.SetPluginEnabled(plugin.PluginName, plugin.IsEnabledByDefault);
            Logger.Info($"Plugin {plugin.PluginName} is Enabled: {plugin.IsEnabledByDefault}");
            return plugin.IsEnabledByDefault;
        }

        private void OnSessionStarted(object sender, DataEventArgs args)
        {
            _visitors.ForEach(x => x.Reset());
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