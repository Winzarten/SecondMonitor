namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using SecondMonitor.Timing.Presentation.ViewModel.Commands;
    using Properties;
    using Model;

    public class ReportingSettingsViewModel : DependencyObject, INotifyPropertyChanged
    {

        private static readonly  DependencyProperty ExportDirectoryProperty = DependencyProperty.Register("ExportDirectory", typeof(string), typeof(ReportingSettingsViewModel), new PropertyMetadata(){PropertyChangedCallback = PropertyChangedCallback});

        private static readonly DependencyProperty MaximumReportsProperty = DependencyProperty.Register("MaximumReports", typeof(int), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty PracticeReportSettingsViewProperty = DependencyProperty.Register("PracticeReportSettingsView", typeof(SessionReportSettingsViewModel), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty QualificationReportSettingsProperty = DependencyProperty.Register("QualificationReportSettings", typeof(SessionReportSettingsViewModel), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty RaceReportSettingsViewProperty = DependencyProperty.Register("RaceReportSettingsView", typeof(SessionReportSettingsViewModel), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty WarmUpReportSettingsViewProperty = DependencyProperty.Register("WarmUpReportSettingsView", typeof(SessionReportSettingsViewModel), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        private static readonly DependencyProperty MinimumSessionLengthProperty = DependencyProperty.Register("MinimumSessionLength", typeof(int), typeof(ReportingSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        public ReportingSettingsViewModel()
        {
            SelectExportDirCommand = new NoArgumentCommand(SelectExportDir, () => true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ExportDirectory
        {
            get => (string)GetValue(ExportDirectoryProperty);
            set
            {
                SetValue(ExportDirectoryProperty, value);
                OnPropertyChanged(nameof(ExportDirectoryReplacedSpecialDirs));
            }
        }

        public string ExportDirectoryReplacedSpecialDirs => ReplaceSpecialDirs((string)GetValue(ExportDirectoryProperty));

        public int MaximumReports
        {
            get => (int)GetValue(MaximumReportsProperty);
            set => SetValue(MaximumReportsProperty, value);
        }

        public SessionReportSettingsViewModel PracticeReportSettingsView
        {
            get => (SessionReportSettingsViewModel)GetValue(PracticeReportSettingsViewProperty);
            set => SetValue(PracticeReportSettingsViewProperty, value);
        }

        public SessionReportSettingsViewModel QualificationReportSettingView
        {
            get => (SessionReportSettingsViewModel)GetValue(QualificationReportSettingsProperty);
            set => SetValue(QualificationReportSettingsProperty, value);
        }

        public SessionReportSettingsViewModel RaceReportSettingsView
        {
            get => (SessionReportSettingsViewModel)GetValue(RaceReportSettingsViewProperty);
            set => SetValue(RaceReportSettingsViewProperty, value);
        }

        public SessionReportSettingsViewModel WarmUpReportSettingsView
        {
            get => (SessionReportSettingsViewModel)GetValue(WarmUpReportSettingsViewProperty);
            set => SetValue(WarmUpReportSettingsViewProperty, value);
        }

        public int MinimumSessionLength
        {
            get => (int)GetValue(MinimumSessionLengthProperty);
            set => SetValue(MinimumSessionLengthProperty, value);
        }

        public void FromModel(ReportingSettings model)
        {
            ExportDirectory = model.ExportDirectory;
            MaximumReports = model.MaximumReports;
            MinimumSessionLength = model.MinimumSessionLength;
            PracticeReportSettingsView = SessionReportSettingsViewModel.FromModel(model.PracticeReportSettings);
            QualificationReportSettingView = SessionReportSettingsViewModel.FromModel(model.QualificationReportSettings);
            WarmUpReportSettingsView = SessionReportSettingsViewModel.FromModel(model.WarmUpReportSettings);
            RaceReportSettingsView = SessionReportSettingsViewModel.FromModel(model.RaceReportSettings);
            CheckExportDirExistence();
        }

        private void CheckExportDirExistence()
        {
            if (string.IsNullOrWhiteSpace(ExportDirectoryReplacedSpecialDirs))
            {
                return;
            }
            if (Directory.Exists(ExportDirectoryReplacedSpecialDirs))
            {
                return;
            }
            Directory.CreateDirectory(ExportDirectoryReplacedSpecialDirs);
        }

        public ICommand SelectExportDirCommand { get; }

        public ReportingSettings ToModel()
        {
            return new ReportingSettings()
                       {
                           ExportDirectory = ExportDirectory,
                           MaximumReports = MaximumReports,
                           PracticeReportSettings = PracticeReportSettingsView.ToModel(),
                           QualificationReportSettings = QualificationReportSettingView.ToModel(),
                           RaceReportSettings = RaceReportSettingsView.ToModel(),
                           WarmUpReportSettings = WarmUpReportSettingsView.ToModel(),
                           MinimumSessionLength = MinimumSessionLength
                       };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectExportDir()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = ExportDirectoryReplacedSpecialDirs;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ExportDirectory = fbd.SelectedPath;
                }
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReportingSettingsViewModel reportingSettingsModelView)
            {
                reportingSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }

        private static string ReplaceSpecialDirs(string path)
        {
            path = path.Replace(@"%MyDocuments%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            return path;
        }
    }
}