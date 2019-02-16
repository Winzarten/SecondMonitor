namespace SecondMonitor.Remote.Application.Controller
{
    using Contracts.NInject;
    using PluginManager.Core;
    using PluginsConfiguration.Common.Controller;

    public class DataBroadcasterPluginController : ISecondMonitorPlugin
    {
        private IPluginSettingsProvider _pluginSettingsProvider;

        public PluginsManager PluginManager { get; set; }

        public string PluginName => "Data Broadcaster";

        public bool IsEnabledByDefault => false;

        public bool IsDaemon => false;

        public void RunPlugin()
        {
            InitializeInjectedProperties();
        }

        private void InitializeInjectedProperties()
        {
            _pluginSettingsProvider = KernelWrapper.Instance.Get<IPluginSettingsProvider>();
        }
    }
}