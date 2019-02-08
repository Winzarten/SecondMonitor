namespace SecondMonitor.PluginsConfiguration.Repository
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using DataModel;

    public class AppDatePluginConfigurationRepository : IPluginConfigurationRepository
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\plugins.xml");

        private readonly XmlSerializer _xmlSerializer;

        public AppDatePluginConfigurationRepository()
        {
            _xmlSerializer = new XmlSerializer(typeof(PluginsConfiguration));
        }


        public PluginsConfiguration LoadOrCreateDefault()
        {
            if (!File.Exists(SettingsPath))
            {
                return CreateDefault();
            }

            using (FileStream file = File.Open(SettingsPath, FileMode.Open))
            {
                return _xmlSerializer.Deserialize(file) as PluginsConfiguration;
            }
        }

        public void Save(PluginsConfiguration pluginsConfiguration)
        {
            using (FileStream file = File.Exists(SettingsPath) ? File.Open(SettingsPath, FileMode.Truncate) : File.Create(SettingsPath))
            {
                _xmlSerializer.Serialize(file, pluginsConfiguration);
            }
        }

        private PluginsConfiguration CreateDefault()
        {
            return new PluginsConfiguration()
            {
                RemoteConfiguration = new RemoteConfiguration()
                {
                    IpAddress = string.Empty,
                    IsBroadcastEnabled = true,
                    Port = 52642,
                }
            };
        }
    }
}