namespace SecondMonitor.ViewModels.TrackInfo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.ViewModels.Annotations;

    public class TrackInfoViewModel : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {
        private string _trackName;
        private string _sessionType;
        private string _rainIntensity;
        private string _airTemperatureInfo;
        private string _trackTemperatureInfo;
        private bool _weatherInfoAvailable;

        private TemperatureUnits _temperatureUnits;

        private Temperature _lastAirTemperature;
        private Temperature _lastTrackTemperature;

        public TrackInfoViewModel()
        {
            Reset();
        }

        public string TrackName
        {
            get => _trackName;
            set
            {
                if (_trackName == value)
                {
                    return;
                }

                _trackName = value;
                NotifyPropertyChanged();
            }
        }

        public string SessionType
        {
            get => _sessionType;
            set
            {
                if (_sessionType == value)
                {
                    return;
                }

                _sessionType = value;
                NotifyPropertyChanged();
            }
        }

        public string RainIntensity
        {
            get => _rainIntensity;
            set
            {
                _rainIntensity = value;
                NotifyPropertyChanged();
            }
        }

        public string AirTemperatureInfo
        {
            get => _airTemperatureInfo;
            set
            {
                if (_airTemperatureInfo == value)
                {
                    return;
                }

                _airTemperatureInfo = value;
                NotifyPropertyChanged();
            }
        }

        public string TrackTemperatureInfo
        {
            get => _trackTemperatureInfo;
            set
            {
                if (_trackTemperatureInfo == value)
                {
                    return;
                }

                _trackTemperatureInfo = value;
                NotifyPropertyChanged();
            }
        }

        public TemperatureUnits TemperatureUnits
        {
            get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                RefreshTemperatures();
            }
        }

        public bool WeatherInfoAvailable
        {
            get => _weatherInfoAvailable;
            set
            {
                if (_weatherInfoAvailable == value)
                {
                    return;
                }

                _weatherInfoAvailable = value;
                NotifyPropertyChanged();
            }
        }


        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.SessionInfo?.TrackInfo == null || dataSet.SessionInfo?.WeatherInfo == null)
            {
                return;
            }

            _lastTrackTemperature = dataSet.SessionInfo.WeatherInfo.TrackTemperature;
            _lastAirTemperature = dataSet.SessionInfo.WeatherInfo.AirTemperature;
            SessionType = dataSet.SessionInfo.SessionType.ToString();
            FormatTrackName(dataSet.SessionInfo.TrackInfo.TrackName, dataSet.SessionInfo.TrackInfo.TrackLayoutName);
            RainIntensity = dataSet.SessionInfo.WeatherInfo.RainIntensity + "%";
            RefreshTemperatures();
        }

        private void FormatTrackName(string trackName, string trackLayout)
        {
            if (string.IsNullOrEmpty(trackLayout))
            {
                TrackName = trackName;
                return;
            }

            TrackName = trackName + " (" + trackLayout + " )";
        }

        private void RefreshTemperatures()
        {
            if (_lastAirTemperature == null || _lastTrackTemperature == null)
            {
                return;
            }

            WeatherInfoAvailable =
                _lastTrackTemperature != Temperature.Zero || _lastTrackTemperature != Temperature.Zero;

            AirTemperatureInfo = _lastAirTemperature.GetFormattedWithUnits(1, TemperatureUnits);
            TrackTemperatureInfo = _lastTrackTemperature.GetFormattedWithUnits(1, TemperatureUnits);
        }

        public void Reset()
        {
            _trackName = "No Track";
            _sessionType = "No Session";
            _rainIntensity = "0%";
            _airTemperatureInfo = string.Empty;
            _trackTemperatureInfo = string.Empty;
            _weatherInfoAvailable = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}