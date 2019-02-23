namespace SecondMonitor.ViewModels.PluginsSettings
{
    using PluginsConfiguration.Common.DataModel;

    public interface IRemoteConfigurationViewModel : IViewModel<RemoteConfiguration>
    {
        string IpAddress { get; set; }

        bool IsFindInLanEnabled { get; set; }

        int Port { get; set; }

        IBroadcastLimitSettingsViewModel BroadcastLimitSettingsViewModel { get; }
    }
}