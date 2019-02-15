namespace SecondMonitor.PluginsConfiguration.Common.Repository
{
    using DataModel;

    public interface IPluginConfigurationRepository
    {
        PluginsConfiguration LoadOrCreateDefault();
        void Save(PluginsConfiguration pluginsConfiguration);
    }
}