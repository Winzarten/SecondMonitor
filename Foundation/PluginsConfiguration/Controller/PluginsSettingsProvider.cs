namespace SecondMonitor.PluginsConfiguration.Common.Controller
{
    using System;
    using System.Linq;
    using DataModel;
    using Repository;

    public class PluginsSettingsProvider : IPluginSettingsProvider
    {
        private readonly IPluginConfigurationRepository _pluginConfigurationRepository;
        private readonly Lazy<PluginsConfiguration> _pluginsConfiguration;

        public PluginsSettingsProvider(IPluginConfigurationRepository pluginConfigurationRepository)
        {
            _pluginConfigurationRepository = pluginConfigurationRepository;
            _pluginsConfiguration = new Lazy<PluginsConfiguration>(LoadPluginsConfiguration);
        }

        public bool TryIsPluginEnabled(string pluginName, out bool isEnabled)
        {
            PluginConfiguration pluginConfiguration = _pluginsConfiguration.Value.PluginsConfigurations.FirstOrDefault(x => x.PluginName == pluginName);
            if (pluginConfiguration == null)
            {
                isEnabled = false;
                return false;
            }

            isEnabled = pluginConfiguration.IsEnabled;
            return true;
        }

        public void SetPluginEnabled(string pluginName, bool isPluginEnabled)
        {
            PluginConfiguration pluginConfiguration = _pluginsConfiguration.Value.PluginsConfigurations.FirstOrDefault(x => x.PluginName == pluginName);
            if (pluginConfiguration == null)
            {
                pluginConfiguration = new PluginConfiguration()
                {
                    PluginName = pluginName
                };
                _pluginsConfiguration.Value.PluginsConfigurations.Add(pluginConfiguration);
            }

            pluginConfiguration.IsEnabled = isPluginEnabled;
            _pluginConfigurationRepository.Save(_pluginsConfiguration.Value);
        }

        private PluginsConfiguration LoadPluginsConfiguration()
        {
            return _pluginConfigurationRepository.LoadOrCreateDefault();
        }
    }
}