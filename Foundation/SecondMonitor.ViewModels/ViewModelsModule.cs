namespace SecondMonitor.ViewModels
{
    using Factory;
    using Ninject.Modules;
    using PluginsSettings;

    public class ViewModelsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IViewModelFactory>().To<ViewModelFactory>();
            Bind<IBroadcastLimitSettingsViewModel>().To<BroadcastLimitSettingsViewModel>();
            Bind<IPluginConfigurationViewModel>().To<PluginConfigurationViewModel>();
            Bind<IPluginsConfigurationViewModel>().To<PluginsConfigurationViewModel>();
            Bind<IRemoteConfigurationViewModel>().To<RemoteConfigurationViewModel>();
        }
    }
}