namespace SecondMonitor.PluginsConfiguration.Common.DataModel
{
    using System.Collections.Generic;

    public class PluginsConfiguration
    {
        public PluginsConfiguration()
        {
            PluginsConfigurations = new List<PluginConfiguration>();
        }

        public RemoteConfiguration RemoteConfiguration { get; set; }
        public List<PluginConfiguration> PluginsConfigurations { get; set; }
    }
}