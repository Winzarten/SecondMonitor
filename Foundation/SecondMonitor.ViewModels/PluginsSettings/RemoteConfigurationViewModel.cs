namespace SecondMonitor.ViewModels.PluginsSettings
{
    using System.Net;
    using System.Net.Sockets;
    using PluginsConfiguration.Common.DataModel;

    public class RemoteConfigurationViewModel : AbstractViewModel<RemoteConfiguration>, IRemoteConfigurationViewModel
    {
        private string _ipAddress;
        private bool _isBroadcastEnabled;
        private int _port;

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