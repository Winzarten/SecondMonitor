namespace SecondMonitor.ViewModels.PluginsSettings
{
    using System.Net;
    using System.Net.Sockets;
    using PluginsConfiguration.Common.DataModel;

    public class RemoteConfigurationViewModel : AbstractViewModel<RemoteConfiguration>, IRemoteConfigurationViewModel
    {
        private string _ipAddress;
        private bool _isFindInLanEnabled;
        private int _port;
        private bool _isRemoteConnectorEnabled;

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (IsValidIpAddress(value))
                {
                    SetProperty(ref _ipAddress, value);
                }
            }
        }

        public bool IsFindInLanEnabled
        {
            get => _isFindInLanEnabled;
            set => SetProperty(ref _isFindInLanEnabled, value);

        }

        public bool IsRemoteConnectorEnabled
        {
            get => _isRemoteConnectorEnabled;
            set => SetProperty(ref _isRemoteConnectorEnabled, value);

        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        protected override void ApplyModel(RemoteConfiguration model)
        {
            IpAddress = model.IpAddress;
            IsFindInLanEnabled = model.IsFindInLanEnabled;
            Port = model.Port;
            IsRemoteConnectorEnabled = model.IsRemoteConnectorEnabled;
        }

        public override RemoteConfiguration SaveToNewModel()
        {
            return new RemoteConfiguration()
            {
                IpAddress = IpAddress,
                IsFindInLanEnabled = IsFindInLanEnabled,
                Port = Port,
                IsRemoteConnectorEnabled = IsRemoteConnectorEnabled
            };
        }

        private static bool IsValidIpAddress(string value)
        {
            if (IPAddress.TryParse(value, out IPAddress ipAddress))
            {
                return ipAddress.AddressFamily == AddressFamily.InterNetwork;
            }

            return false;
        }
    }
}