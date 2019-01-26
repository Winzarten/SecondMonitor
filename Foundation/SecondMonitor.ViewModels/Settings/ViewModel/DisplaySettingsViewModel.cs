namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WindowsControls.WPF.Commands;
    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;
    using Model;
    using Properties;

    public class DisplaySettingsViewModel : INotifyPropertyChanged
    {
        private VelocityUnits _velocityUnits;
        private TemperatureUnits _temperatureUnits;
        private PressureUnits _pressureUnits;
        private VolumeUnits _volumeUnits;
        private FuelCalculationScope _fuelCalculationScope;
        private int _paceLaps;
        private int _refreshRate;
        private bool _scrollToPlayer;
        private SessionOptionsViewModel _practiceSessionDisplayOptionsView;
        private SessionOptionsViewModel _qualificationSessionDisplayOptionsView;
        private SessionOptionsViewModel _raceSessionDisplayOptionsView;
        private ReportingSettingsViewModel _reportingSettingsView;
        private bool _animateDriverPosition;
        private bool _animateDeltaTimes;
        private MapDisplaySettingsViewModel _mapDisplaySettingsViewModel;
        private TelemetrySettingsViewModel _telemetrySettingsViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenLogDirectoryCommand => new RelayCommand(OpenLogDirectory);

        public TelemetrySettingsViewModel TelemetrySettingsViewModel
        {
            get => _telemetrySettingsViewModel;
            set
            {
                _telemetrySettingsViewModel = value;
                OnPropertyChanged();
            }
        }

        public TemperatureUnits TemperatureUnits
        {
            get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                OnPropertyChanged();
            }
        }

        public PressureUnits PressureUnits
        {
            get => _pressureUnits;
            set
            {
                _pressureUnits = value;
                OnPropertyChanged();
            }
        }

        public VolumeUnits VolumeUnits
        {
            get => _volumeUnits;
            set
            {
                _volumeUnits = value;
                OnPropertyChanged();
            }
        }

        public VelocityUnits VelocityUnits
        {
            get => _velocityUnits;
            set
            {
                _velocityUnits = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DistanceUnits));
                OnPropertyChanged(nameof(FuelPerDistanceUnits));
            }
        }

        public FuelCalculationScope FuelCalculationScope
        {
            get => _fuelCalculationScope;
            set
            {
                _fuelCalculationScope = value;
                OnPropertyChanged();
            }
        }

        public DistanceUnits DistanceUnits
        {
            get
            {
                switch (VelocityUnits)
                {
                    case VelocityUnits.Kph:
                        return DistanceUnits.Kilometers;
                    case VelocityUnits.Mph:
                        return DistanceUnits.Miles;
                    case VelocityUnits.Ms:
                        return DistanceUnits.Meters;
                    default:
                        return DistanceUnits.Kilometers;
                }
            }
        }

        public DistanceUnits DistanceUnitsSmall
        {
            get
            {
                switch (VelocityUnits)
                {
                    case VelocityUnits.Kph:
                        return DistanceUnits.Meters;
                    case VelocityUnits.Mph:
                        return DistanceUnits.Yards;
                    case VelocityUnits.Ms:
                        return DistanceUnits.Meters;
                    default:
                        return DistanceUnits.Meters;
                }
            }
        }

        public DistanceUnits DistanceUnitsVerySmall
        {
            get
            {
                switch (VelocityUnits)
                {
                    case VelocityUnits.Kph:
                        return DistanceUnits.Centimeter;
                    case VelocityUnits.Mph:
                        return DistanceUnits.Inches;
                    case VelocityUnits.Ms:
                        return DistanceUnits.Centimeter;
                    default:
                        return DistanceUnits.Centimeter;
                }
            }
        }

        public FuelPerDistanceUnits FuelPerDistanceUnits
        {
            get
            {
                switch (VelocityUnits)
                {
                    case VelocityUnits.Kph:
                        return FuelPerDistanceUnits.LitersPerHundredKm;
                    case VelocityUnits.Mph:
                        return FuelPerDistanceUnits.MilesPerGallon;
                    case VelocityUnits.Ms:
                        return FuelPerDistanceUnits.LitersPerHundredKm;
                    default:
                        return FuelPerDistanceUnits.LitersPerHundredKm;
                }
            }
        }

        public int PaceLaps
        {
            get => _paceLaps;
            set
            {
                _paceLaps = value;
                OnPropertyChanged();
            }
        }

        public int RefreshRate
        {
            get => _refreshRate;
            set
            {
                _refreshRate = value;
                OnPropertyChanged();
            }
        }

        public bool ScrollToPlayer
        {
            get => _scrollToPlayer;
            set
            {
                _scrollToPlayer = value;
                OnPropertyChanged();
            }
        }

        public bool AnimateDriversPosition
        {
            get => _animateDriverPosition;
            set
            {
                _animateDriverPosition = value;
                OnPropertyChanged();
            }
        }

        public bool AnimateDeltaTimes
        {
            get => _animateDeltaTimes;
            set
            {
                _animateDeltaTimes = value;
                OnPropertyChanged();
            }
        }

        public SessionOptionsViewModel PracticeSessionDisplayOptionsView
        {
            get => _practiceSessionDisplayOptionsView;
            set
            {
                _practiceSessionDisplayOptionsView = value;
                OnPropertyChanged();
            }
        }

        public SessionOptionsViewModel QualificationSessionDisplayOptionsView
        {
            get => _qualificationSessionDisplayOptionsView;
            set
            {
                _qualificationSessionDisplayOptionsView = value;
                OnPropertyChanged();
            }
        }

        public SessionOptionsViewModel RaceSessionDisplayOptionsView
        {
            get => _raceSessionDisplayOptionsView;
            set
            {
                _raceSessionDisplayOptionsView = value;
                OnPropertyChanged();
            }
        }

        public ReportingSettingsViewModel ReportingSettingsView
        {
            get => _reportingSettingsView;
            set
            {
                _reportingSettingsView = value;
                OnPropertyChanged();
            }
        }

        public MapDisplaySettingsViewModel MapDisplaySettingsViewModel
        {
            get => _mapDisplaySettingsViewModel;
            set
            {
                _mapDisplaySettingsViewModel = value;
                OnPropertyChanged();
            }
        }

        public void FromModel(DisplaySettings settings)
        {
            TemperatureUnits = settings.TemperatureUnits;
            PressureUnits = settings.PressureUnits;
            VolumeUnits = settings.VolumeUnits;
            VelocityUnits = settings.VelocityUnits;
            FuelCalculationScope = settings.FuelCalculationScope;
            PaceLaps = settings.PaceLaps;
            RefreshRate = settings.RefreshRate;
            ScrollToPlayer = settings.ScrollToPlayer;
            AnimateDeltaTimes = settings.AnimateDeltaTimes;
            AnimateDriversPosition = settings.AnimateDriversPosition;

            MapDisplaySettingsViewModel = new MapDisplaySettingsViewModel();
            MapDisplaySettingsViewModel.FromModel(settings.MapDisplaySettings);

            PracticeSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.PracticeOptions);
            QualificationSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.QualificationOptions);
            RaceSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.RaceOptions);

            ReportingSettingsView = new ReportingSettingsViewModel();
            ReportingSettingsView.FromModel(settings.ReportingSettings);

            TelemetrySettingsViewModel = new TelemetrySettingsViewModel();
            TelemetrySettingsViewModel.FromModel(settings.TelemetrySettings);
        }

        public DisplaySettings ToModel()
        {
            return new DisplaySettings()
            {
                TemperatureUnits = TemperatureUnits,
                PressureUnits = PressureUnits,
                VolumeUnits = VolumeUnits,
                VelocityUnits =  VelocityUnits,
                FuelCalculationScope = FuelCalculationScope,
                PaceLaps = PaceLaps,
                RefreshRate = RefreshRate,
                ScrollToPlayer = ScrollToPlayer,
                PracticeOptions = PracticeSessionDisplayOptionsView.ToModel(),
                QualificationOptions = QualificationSessionDisplayOptionsView.ToModel(),
                RaceOptions = RaceSessionDisplayOptionsView.ToModel(),
                ReportingSettings = ReportingSettingsView.ToModel(),
                AnimateDriversPosition =  AnimateDriversPosition,
                AnimateDeltaTimes =  AnimateDeltaTimes,
                MapDisplaySettings = MapDisplaySettingsViewModel.SaveToNewModel(),
                TelemetrySettings = TelemetrySettingsViewModel.SaveToNewModel()
            };
        }

        private void OpenLogDirectory()
        {
            string reportDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecondMonitor");
            Task.Run(
                () =>
                {
                    Process.Start(reportDirectory);
                });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
