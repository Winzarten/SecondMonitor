namespace SecondMonitor.PluginsConfiguration.Repository
{
    using DataModel;

    public interface IPluginConfigurationRepository
    {
        PluginsConfiguration LoadOrCreateDefault();
        void Save(PluginsConfiguration pluginsConfiguration);
    }
}