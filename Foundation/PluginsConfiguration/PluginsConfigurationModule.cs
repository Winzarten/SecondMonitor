namespace SecondMonitor.PluginsConfiguration
{
    using Controller;
    using Ninject.Modules;
    using Repository;

    public class PluginsConfigurationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPluginConfigurationRepository>().To<AppDatePluginConfigurationRepository>();
            Bind<IPluginSettingsProvider>().To<PluginsSettingsProvider>();
        }
    }
}