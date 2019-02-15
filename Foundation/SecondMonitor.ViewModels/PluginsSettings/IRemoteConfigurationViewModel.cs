namespace SecondMonitor.ViewModels.PluginsSettings
{
    using PluginsConfiguration.Common.DataModel;

    public interface IRemoteConfigurationViewModel : IViewModel<RemoteConfiguration>
    {
        string IpAddress { get; set; }

        bool IsBroadcastEnabled { get; set; }

        int Port { get; set; }
    }
}