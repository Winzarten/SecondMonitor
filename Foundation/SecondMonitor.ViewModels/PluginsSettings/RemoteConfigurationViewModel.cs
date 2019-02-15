namespace SecondMonitor.ViewModels.PluginsSettings
{
    using PluginsConfiguration.Common.DataModel;

    public class RemoteConfigurationViewModel : AbstractViewModel<RemoteConfiguration>, IRemoteConfigurationViewModel
    {
        private string _ipAddress;
        private bool _isBroadcastEnabled;
        private int _port;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public bool IsBroadcastEnabled
        {
            get => _isBroadcastEnabled;
            set => SetProperty(ref _isBroadcastEnabled, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        protected override void ApplyModel(RemoteConfiguration model)
        {
            IpAddress = model.IpAddress;
            IsBroadcastEnabled = model.IsBroadcastEnabled;
            Port = model.Port;
        }

        public override RemoteConfiguration SaveToNewModel()
        {
            return new RemoteConfiguration()
            {
                IpAddress = IpAddress,
                IsBroadcastEnabled = IsBroadcastEnabled,
                Port = Port
            };
        }
    }
}