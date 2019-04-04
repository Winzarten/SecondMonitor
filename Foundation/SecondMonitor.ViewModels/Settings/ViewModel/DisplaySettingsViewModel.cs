using SecondMonitor.DataModel.BasicProperties.Units;

namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Contracts.Commands;
    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;
    using Model;
    using Properties;

    public class DisplaySettingsViewModel : AbstractViewModel<DisplaySettings>
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
        private MultiClassDisplayKind _multiClassDisplayKind;
        private ForceUnits _forceUnits;

        public ICommand OpenLogDirectoryCommand => new RelayCommand(OpenLogDirectory);

        public TelemetrySettingsViewModel TelemetrySettingsViewModel
        {
            get => _telemetrySettingsViewModel;
            set
            {
                _telemetrySettingsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public TemperatureUnits TemperatureUnits
        {
            get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public MultiClassDisplayKind MultiClassDisplayKind
        {
            get => _multiClassDisplayKind;
            set
            {
                _multiClassDisplayKind = value;
                NotifyPropertyChanged();
            }
        }

        public PressureUnits PressureUnits
        {
            get => _pressureUnits;
            set
            {
                _pressureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public VolumeUnits VolumeUnits
        {
            get => _volumeUnits;
            set
            {
                _volumeUnits = value;
                NotifyPropertyChanged();
            }
        }

        public VelocityUnits VelocityUnits
        {
            get => _velocityUnits;
            set
            {
                _velocityUnits = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(DistanceUnits));
                NotifyPropertyChanged(nameof(FuelPerDistanceUnits));
            }
        }

        public ForceUnits ForceUnits
        {
            get => _forceUnits;
            set => SetProperty(ref _forceUnits, value);
        }

        public FuelCalculationScope FuelCalculationScope
        {
            get => _fuelCalculationScope;
            set
            {
                _fuelCalculationScope = value;
                NotifyPropertyChanged();
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

        public VelocityUnits VelocityUnitsVerySmall
        {
            get
            {
                switch (VelocityUnits)
                {
                    case VelocityUnits.Kph:
                        return VelocityUnits.CmPerSecond;
                    case VelocityUnits.Mph:
                        return VelocityUnits.InPerSecond;
                    case VelocityUnits.Ms:
                        return VelocityUnits.CmPerSecond;
                    case VelocityUnits.Fps:
                        return VelocityUnits.InPerSecond;
                    case VelocityUnits.CmPerSecond:
                        return VelocityUnits.CmPerSecond;
                    case VelocityUnits.InPerSecond:
                        return VelocityUnits.InPerSecond;
                    default:
                        return VelocityUnits.CmPerSecond;
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
                NotifyPropertyChanged();
            }
        }

        public int RefreshRate
        {
            get => _refreshRate;
            set
            {
                _refreshRate = value;
                NotifyPropertyChanged();
            }
        }

        public bool ScrollToPlayer
        {
            get => _scrollToPlayer;
            set
            {
                _scrollToPlayer = value;
                NotifyPropertyChanged();
            }
        }

        public bool AnimateDriversPosition
        {
            get => _animateDriverPosition;
            set
            {
                _animateDriverPosition = value;
                NotifyPropertyChanged();
            }
        }

        public bool AnimateDeltaTimes
        {
            get => _animateDeltaTimes;
            set
            {
                _animateDeltaTimes = value;
                NotifyPropertyChanged();
            }
        }

        public SessionOptionsViewModel PracticeSessionDisplayOptionsView
        {
            get => _practiceSessionDisplayOptionsView;
            set
            {
                _practiceSessionDisplayOptionsView = value;
                NotifyPropertyChanged();
            }
        }

        public SessionOptionsViewModel QualificationSessionDisplayOptionsView
        {
            get => _qualificationSessionDisplayOptionsView;
            set
            {
                _qualificationSessionDisplayOptionsView = value;
                NotifyPropertyChanged();
            }
        }

        public SessionOptionsViewModel RaceSessionDisplayOptionsView
        {
            get => _raceSessionDisplayOptionsView;
            set
            {
                _raceSessionDisplayOptionsView = value;
                NotifyPropertyChanged();
            }
        }

        public ReportingSettingsViewModel ReportingSettingsView
        {
            get => _reportingSettingsView;
            set
            {
                _reportingSettingsView = value;
                NotifyPropertyChanged();
            }
        }

        public MapDisplaySettingsViewModel MapDisplaySettingsViewModel
        {
            get => _mapDisplaySettingsViewModel;
            set
            {
                _mapDisplaySettingsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        protected override void ApplyModel(DisplaySettings settings)
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
            MultiClassDisplayKind = settings.MultiClassDisplayKind;
            ForceUnits = settings.ForceUnits;

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

        public override DisplaySettings SaveToNewModel()
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
                TelemetrySettings = TelemetrySettingsViewModel.SaveToNewModel(),
                MultiClassDisplayKind = MultiClassDisplayKind,
                ForceUnits = ForceUnits,
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
    }
}
