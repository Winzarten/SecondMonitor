namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.Timing.Properties;
    using SecondMonitor.Timing.Settings.Model;

    public class ReportingSettingsModelView : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly  DependencyProperty ExportDirectoryProperty = DependencyProperty.Register("ExportDirectory", typeof(string), typeof(ReportingSettingsModelView), new PropertyMetadata(){PropertyChangedCallback = PropertyChangedCallback});

        private static readonly DependencyProperty MaximumReportsProperty = DependencyProperty.Register("MaximumReports", typeof(int), typeof(ReportingSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty PracticeReportSettingsProperty = DependencyProperty.Register("PracticeReportSettings", typeof(SessionReportSettingsModelView), typeof(ReportingSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty QualificationReportSettingsProperty = DependencyProperty.Register("QualificationReportSettings", typeof(SessionReportSettingsModelView), typeof(ReportingSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty RaceReportSettingsProperty = DependencyProperty.Register("RaceReportSettings", typeof(SessionReportSettingsModelView), typeof(ReportingSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty WarmUpReportSettingsProperty = DependencyProperty.Register("WarmUpReportSettings", typeof(SessionReportSettingsModelView), typeof(ReportingSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        public string ExportDirectory
        {
            get => (string)GetValue(ExportDirectoryProperty);
            set => SetValue(ExportDirectoryProperty, value);
        }

        public int MaximumReports
        {
            get => (int)GetValue(MaximumReportsProperty);
            set => SetValue(MaximumReportsProperty, value);
        }

        public SessionReportSettingsModelView PracticeReportSettings
        {
            get => (SessionReportSettingsModelView)GetValue(PracticeReportSettingsProperty);
            set => SetValue(PracticeReportSettingsProperty, value);
        }

        public SessionReportSettingsModelView QualificationReportSetting
        {
            get => (SessionReportSettingsModelView)GetValue(QualificationReportSettingsProperty);
            set => SetValue(QualificationReportSettingsProperty, value);
        }

        public SessionReportSettingsModelView RaceReportSettings
        {
            get => (SessionReportSettingsModelView)GetValue(RaceReportSettingsProperty);
            set => SetValue(RaceReportSettingsProperty, value);
        }
        
        public SessionReportSettingsModelView WarmUpReportSettings
        {
            get => (SessionReportSettingsModelView)GetValue(WarmUpReportSettingsProperty);
            set => SetValue(WarmUpReportSettingsProperty, value);
        }

        public void FromModel(ReportingSettings model)
        {
            ExportDirectory = model.ExportDirectory;
            MaximumReports = model.MaximumReports;
            PracticeReportSettings = SessionReportSettingsModelView.FromModel(model.PracticeReportSettings);
            QualificationReportSetting = SessionReportSettingsModelView.FromModel(model.QualificationReportSettings);
            WarmUpReportSettings = SessionReportSettingsModelView.FromModel(model.WarmUpReportSettings);
            RaceReportSettings = SessionReportSettingsModelView.FromModel(model.RaceReportSettings);
        }

        public ReportingSettings ToModel()
        {
            return new ReportingSettings()
                       {
                           ExportDirectory = ExportDirectory,
                           MaximumReports = MaximumReports,
                           PracticeReportSettings = PracticeReportSettings.ToModel(),
                           QualificationReportSettings = QualificationReportSetting.ToModel(),
                           RaceReportSettings = RaceReportSettings.ToModel(),
                           WarmUpReportSettings = WarmUpReportSettings.ToModel()
                       };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReportingSettingsModelView reportingSettingsModelView)
            {
                reportingSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }
    }
}