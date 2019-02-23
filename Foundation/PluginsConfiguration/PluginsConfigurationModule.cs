namespace SecondMonitor.PluginsConfiguration.Common
{
    using Controller;
    using Ninject.Modules;
    using Repository;

    public class PluginsConfigurationModule : NinjectModule
    {
        public override void Load()
        {
            Rebind<IPluginConfigurationRepository>().To<AppDataPluginConfigurationRepository>();
            Rebind<IPluginSettingsProvider>().To<PluginsSettingsProvider>();
        }
    }
}