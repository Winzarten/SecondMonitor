namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;
    using Properties;
    using Model;

    public class DisplaySettingsViewModel : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty VelocityUnitsProperty = DependencyProperty.Register("VelocityUnits", typeof(VelocityUnits), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty FuelCalculationScopeProperty = DependencyProperty.Register("FuelCalculationScope", typeof(FuelCalculationScope), typeof(DisplaySettingsViewModel), new PropertyMetadata{ PropertyChangedCallback = PropertyChangedCallback});
        public static readonly DependencyProperty PaceLapsProperty = DependencyProperty.Register("PaceLaps", typeof(int), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty RefreshRateProperty = DependencyProperty.Register("RefreshRate", typeof(int), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ScrollToPlayerProperty = DependencyProperty.Register("ScrollToPlayer", typeof(bool), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty PracticeSessionDisplayOptionsViewProperty = DependencyProperty.Register("PracticeSessionDisplayOptionsView", typeof(SessionOptionsViewModel), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty QualificationSessionDisplayOptionsViewProperty = DependencyProperty.Register("QualificationSessionDisplayOptionsView", typeof(SessionOptionsViewModel), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty RaceSessionDisplayOptionsViewProperty = DependencyProperty.Register("RaceSessionDisplayOptionsView", typeof(SessionOptionsViewModel), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ReportingSettingsViewProperty = DependencyProperty.Register("SettingsView", typeof(ReportingSettingsViewModel), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty AnimateDriversPositionProperty = DependencyProperty.Register("AnimateDriversPosition", typeof(bool), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty AnimateDeltaTimesProperty = DependencyProperty.Register("AnimateDeltaTimes", typeof(bool), typeof(DisplaySettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

        public event PropertyChangedEventHandler PropertyChanged;

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits)GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        public VelocityUnits VelocityUnits
        {
            get => (VelocityUnits)GetValue(VelocityUnitsProperty);
            set => SetValue(VelocityUnitsProperty, value);
        }

        public FuelCalculationScope FuelCalculationScope
        {
            get => (FuelCalculationScope) GetValue(FuelCalculationScopeProperty);
            set => SetValue(FuelCalculationScopeProperty, value);
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
            get => (int) GetValue(PaceLapsProperty);
            set => SetValue(PaceLapsProperty, value);
        }

        public int RefreshRate
        {
            get => (int) GetValue(RefreshRateProperty);
            set => SetValue(RefreshRateProperty, value);
        }

        public bool ScrollToPlayer
        {
            get => (bool) GetValue(ScrollToPlayerProperty);
            set => SetValue(ScrollToPlayerProperty, value);
        }

        public bool AnimateDriversPosition
        {
            get => (bool)GetValue(AnimateDriversPositionProperty);
            set => SetValue(AnimateDriversPositionProperty, value);
        }

        public bool AnimateDeltaTimes
        {
            get => (bool)GetValue(AnimateDeltaTimesProperty);
            set => SetValue(AnimateDeltaTimesProperty, value);
        }

        public SessionOptionsViewModel PracticeSessionDisplayOptionsView
        {
            get => (SessionOptionsViewModel)GetValue(PracticeSessionDisplayOptionsViewProperty);
            set => SetValue(PracticeSessionDisplayOptionsViewProperty, value);
        }

        public SessionOptionsViewModel QualificationSessionDisplayOptionsView
        {
            get => (SessionOptionsViewModel)GetValue(QualificationSessionDisplayOptionsViewProperty);
            set => SetValue(QualificationSessionDisplayOptionsViewProperty, value);
        }

        public SessionOptionsViewModel RaceSessionDisplayOptionsView
        {
            get => (SessionOptionsViewModel)GetValue(RaceSessionDisplayOptionsViewProperty);
            set => SetValue(RaceSessionDisplayOptionsViewProperty, value);
        }

        public ReportingSettingsViewModel ReportingSettingsView
        {
            get => (ReportingSettingsViewModel)GetValue(ReportingSettingsViewProperty);
            set => SetValue(ReportingSettingsViewProperty, value);
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

            PracticeSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.PracticeOptions);
            QualificationSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.QualificationOptions);
            RaceSessionDisplayOptionsView = SessionOptionsViewModel.CreateFromModel(settings.RaceOptions);

            ReportingSettingsView = new ReportingSettingsViewModel();
            ReportingSettingsView.FromModel(settings.ReportingSettings);
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
                AnimateDeltaTimes =  AnimateDeltaTimes
            };
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            DisplaySettingsViewModel sender = (DisplaySettingsViewModel) dependencyObject;
            if (dependencyPropertyChangedEventArgs.Property.Name == nameof(VelocityUnits))
            {
                sender.OnPropertyChanged(nameof(DistanceUnits));
                sender.OnPropertyChanged(nameof(FuelPerDistanceUnits));
            }

            sender.OnPropertyChanged(dependencyPropertyChangedEventArgs.Property.Name);
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
