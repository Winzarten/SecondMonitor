using SecondMonitor.ViewModels.Factory;

namespace SecondMonitor.ViewModels.PluginsSettings
{
    using System.Net;
    using System.Net.Sockets;
    using PluginsConfiguration.Common.DataModel;

    public class RemoteConfigurationViewModel : AbstractViewModel<RemoteConfiguration>, IRemoteConfigurationViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private string _ipAddress;
        private bool _isFindInLanEnabled;
        private int _port;
        private bool _isRemoteConnectorEnabled;
        private IBroadcastLimitSettingsViewModel _borBroadcastLimitSettingsViewModel;

        public RemoteConfigurationViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

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

        public IBroadcastLimitSettingsViewModel BroadcastLimitSettingsViewModel
        {
            get => _borBroadcastLimitSettingsViewModel;
            set => SetProperty(ref _borBroadcastLimitSettingsViewModel, value);
        }

        protected override void ApplyModel(RemoteConfiguration model)
        {
            IpAddress = model.IpAddress;
            IsFindInLanEnabled = model.IsFindInLanEnabled;
            Port = model.Port;
            IsRemoteConnectorEnabled = model.IsRemoteConnectorEnabled;

            IBroadcastLimitSettingsViewModel newBroadcastLimitSettingsViewModel =
                _viewModelFactory.Create<IBroadcastLimitSettingsViewModel>();
            newBroadcastLimitSettingsViewModel.FromModel(model.BroadcastLimitSettings);
            BroadcastLimitSettingsViewModel = newBroadcastLimitSettingsViewModel;
        }

        public override RemoteConfiguration SaveToNewModel()
        {
            return new RemoteConfiguration()
            {
                IpAddress = IpAddress,
                IsFindInLanEnabled = IsFindInLanEnabled,
                Port = Port,
                IsRemoteConnectorEnabled = IsRemoteConnectorEnabled,
                BroadcastLimitSettings = BroadcastLimitSettingsViewModel.SaveToNewModel(),
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