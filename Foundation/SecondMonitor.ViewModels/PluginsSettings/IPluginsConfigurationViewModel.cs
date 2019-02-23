namespace SecondMonitor.ViewModels.PluginsSettings
{
    using System.Collections.Generic;
    using PluginsConfiguration.Common.DataModel;

    public interface IPluginsConfigurationViewModel : IViewModel<PluginsConfiguration>
    {
        IRemoteConfigurationViewModel RemoteConfigurationViewModel { get; }

        IReadOnlyCollection<IPluginConfigurationViewModel> PluginConfigurations { get; }
    }
}