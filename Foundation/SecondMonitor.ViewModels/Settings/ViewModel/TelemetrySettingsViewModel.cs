namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using Model;

    public class TelemetrySettingsViewModel : AbstractViewModel<TelemetrySettings>
    {
        private bool _isFeatureEnabled;
        private bool _isTelemetryLoggingEnabled;
        private int _loggingInterval;
        private int _maxSessionsKept;
        private bool _logInvalidLaps;

        public bool IsFeatureEnabled
        {
            get => _isFeatureEnabled;
            set
            {
                _isFeatureEnabled = value;
                NotifyPropertyChanged();
            }
        }


        public bool IsTelemetryLoggingEnabled
        {
            get => _isTelemetryLoggingEnabled;
            set
            {
                _isTelemetryLoggingEnabled = value;
                NotifyPropertyChanged();
            }
        }
        public int LoggingInterval
        {
            get => _loggingInterval;
            set
            {
                _loggingInterval = value;
                NotifyPropertyChanged();
            }
        }

        public int MaxSessionsKept
        {
            get => _maxSessionsKept;
            set
            {
                _maxSessionsKept = value;
                NotifyPropertyChanged();
            }
        }

        public bool LogInvalidLaps
        {
            get => _logInvalidLaps;
            set => SetProperty(ref _logInvalidLaps, value);
        }


        protected override void ApplyModel(TelemetrySettings model)
        {
            IsFeatureEnabled = true;
            IsTelemetryLoggingEnabled = model.IsTelemetryLoggingEnabled;
            LoggingInterval = model.LoggingInterval;
            MaxSessionsKept = model.MaxSessionsKept;
            LogInvalidLaps = model.LogInvalidLaps;
        }

        public override TelemetrySettings SaveToNewModel()
        {
            return new TelemetrySettings()
            {
                IsFeatureEnabled = IsFeatureEnabled,
                IsTelemetryLoggingEnabled = IsTelemetryLoggingEnabled,
                LoggingInterval = LoggingInterval,
                MaxSessionsKept = MaxSessionsKept,
                LogInvalidLaps = LogInvalidLaps,
            };
        }
    }
}