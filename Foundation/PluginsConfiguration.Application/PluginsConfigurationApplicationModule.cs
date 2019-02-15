namespace SecondMonitor.PluginsConfiguration.Application
{
    using Controllers;
    using Ninject.Modules;
    using PluginManager.Core;
    using ViewModels;

    public class PluginsConfigurationApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<PluginsManager>().ToSelf();
            Bind<IPluginConfigurationController>().To<PluginsConfigurationController>();
            Bind<IPluginsSettingsWindowViewModel>().To<PluginsSettingsWindowViewModel>();
        }
    }
}