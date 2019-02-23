using SecondMonitor.PluginsConfiguration.Common.DataModel;

namespace SecondMonitor.ViewModels.PluginsSettings
{
    public class BroadcastLimitSettingsViewModel : AbstractViewModel<BroadcastLimitSettings>, IBroadcastLimitSettingsViewModel
    {
        private bool _isEnabled;
        private int _minimumPackageInterval;
        private int _playerTimingPackageInterval;
        private int _otherDriversTimingPackageInterval;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public int MinimumPackageInterval
        {
            get => _minimumPackageInterval;
            set => SetProperty(ref _minimumPackageInterval, value);
        }

        public int PlayerTimingPackageInterval
        {
            get => _playerTimingPackageInterval;
            set => SetProperty(ref _playerTimingPackageInterval, value);
        }

        public int OtherDriversTimingPackageInterval
        {
            get => _otherDriversTimingPackageInterval;
            set => SetProperty(ref _otherDriversTimingPackageInterval, value);
        }

        protected override void ApplyModel(BroadcastLimitSettings model)
        {
            IsEnabled = model.IsEnabled;
            MinimumPackageInterval = model.MinimumPackageInterval;
            PlayerTimingPackageInterval = model.PlayerTimingPackageInterval;
            OtherDriversTimingPackageInterval = model.OtherDriversTimingPackageInterval;
        }

        public override BroadcastLimitSettings SaveToNewModel()
        {
            return new BroadcastLimitSettings()
            {
                IsEnabled = IsEnabled,
                MinimumPackageInterval = MinimumPackageInterval,
                OtherDriversTimingPackageInterval = OtherDriversTimingPackageInterval,
                PlayerTimingPackageInterval = PlayerTimingPackageInterval
            };
        }
    }
}