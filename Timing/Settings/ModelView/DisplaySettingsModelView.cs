namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using DataModel.BasicProperties;
    using Properties;
    using Model;

    public class DisplaySettingsModelView : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty VelocityUnitsProperty = DependencyProperty.Register("VelocityUnits", typeof(VelocityUnits), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty FuelCalculationScopeProperty = DependencyProperty.Register("FuelCalculationScope", typeof(FuelCalculationScope), typeof(DisplaySettingsModelView), new PropertyMetadata{ PropertyChangedCallback = PropertyChangedCallback});
        public static readonly DependencyProperty PaceLapsProperty = DependencyProperty.Register("PaceLaps", typeof(int), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty RefreshRateProperty = DependencyProperty.Register("RefreshRate", typeof(int), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ScrollToPlayerProperty = DependencyProperty.Register("ScrollToPlayer", typeof(bool), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });        
        public static readonly DependencyProperty PracticeSessionDisplayOptionsProperty = DependencyProperty.Register("PracticeSessionDisplayOptions", typeof(SessionOptionsModelView), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty QualificationSessionDisplayOptionsProperty = DependencyProperty.Register("QualificationSessionDisplayOptions", typeof(SessionOptionsModelView), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty RaceSessionDisplayOptionsProperty = DependencyProperty.Register("RaceSessionDisplayOptions", typeof(SessionOptionsModelView), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ReportingSettingsProperty = DependencyProperty.Register("Settings", typeof(ReportingSettingsModelView), typeof(DisplaySettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

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
        
        public SessionOptionsModelView PracticeSessionDisplayOptions
        {
            get => (SessionOptionsModelView)GetValue(PracticeSessionDisplayOptionsProperty);
            set => SetValue(PracticeSessionDisplayOptionsProperty, value);
        }

        public SessionOptionsModelView QualificationSessionDisplayOptions
        {
            get => (SessionOptionsModelView)GetValue(QualificationSessionDisplayOptionsProperty);
            set => SetValue(QualificationSessionDisplayOptionsProperty, value);
        }

        public SessionOptionsModelView RaceSessionDisplayOptions
        {
            get => (SessionOptionsModelView)GetValue(RaceSessionDisplayOptionsProperty);
            set => SetValue(RaceSessionDisplayOptionsProperty, value);
        }

        public ReportingSettingsModelView ReportingSettings
        {
            get => (ReportingSettingsModelView)GetValue(ReportingSettingsProperty);
            set => SetValue(ReportingSettingsProperty, value);
        }

        public void FromModel(DisplaySettings settings)
        {
            TemperatureUnits = settings.TemperatureUnits;
            PressureUnits = settings.PressureUnits;
            VolumeUnits = settings.VolumeUnits;
            FuelCalculationScope = settings.FuelCalculationScope;
            PaceLaps = settings.PaceLaps;
            RefreshRate = settings.RefreshRate;
            ScrollToPlayer = settings.ScrollToPlayer;

            PracticeSessionDisplayOptions = SessionOptionsModelView.CreateFromModel(settings.PracticeOptions);
            QualificationSessionDisplayOptions = SessionOptionsModelView.CreateFromModel(settings.QualificationOptions);
            RaceSessionDisplayOptions = SessionOptionsModelView.CreateFromModel(settings.RaceOptions);

            ReportingSettings = new ReportingSettingsModelView();
            ReportingSettings.FromModel(settings.ReportingSettings);
        }

        public DisplaySettings ToModel()
        {
            return new DisplaySettings()
            {
                TemperatureUnits = TemperatureUnits,
                PressureUnits = PressureUnits,
                VolumeUnits = VolumeUnits,
                FuelCalculationScope = FuelCalculationScope,
                PaceLaps = PaceLaps,
                RefreshRate = RefreshRate,
                ScrollToPlayer = ScrollToPlayer,
                PracticeOptions = PracticeSessionDisplayOptions.ToModel(),
                QualificationOptions = QualificationSessionDisplayOptions.ToModel(),
                RaceOptions = RaceSessionDisplayOptions.ToModel(),
                ReportingSettings = ReportingSettings.ToModel()
            };
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            DisplaySettingsModelView sender = (DisplaySettingsModelView) dependencyObject;
            sender.OnPropertyChanged(dependencyPropertyChangedEventArgs.Property.Name);
        }
        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
