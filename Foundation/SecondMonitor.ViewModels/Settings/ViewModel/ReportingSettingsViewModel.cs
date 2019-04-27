namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Contracts.Commands;
    using Model;

    public class ReportingSettingsViewModel : AbstractViewModel
    {

        public ReportingSettingsViewModel()
        {
            SelectExportDirCommand = new RelayCommand(SelectExportDir);
        }


        private string _exportDirectory;
        internal string ExportDirectory
        {
            get => _exportDirectory;
            set => SetProperty(ref _exportDirectory, value);
        }

        public string ExportDirectoryReplacedSpecialDirs => ReplaceSpecialDirs(ExportDirectory);

        private int _maximumReports;
        public int MaximumReports
        {
            get => _maximumReports;
            set => SetProperty(ref _maximumReports, value);
        }

        private SessionReportSettingsViewModel _practiceReportSettingsView;
        public SessionReportSettingsViewModel PracticeReportSettingsView
        {
            get => _practiceReportSettingsView;
            set => SetProperty(ref _practiceReportSettingsView, value);
        }

        private SessionReportSettingsViewModel _qualificationReportSettingView;
        public SessionReportSettingsViewModel QualificationReportSettingView
        {
            get => _qualificationReportSettingView;
            set => SetProperty(ref _qualificationReportSettingView, value);
        }

        private SessionReportSettingsViewModel _raceReportSettingsView;
        public SessionReportSettingsViewModel RaceReportSettingsView
        {
            get => _raceReportSettingsView;
            set => SetProperty(ref _raceReportSettingsView, value);
        }

        private SessionReportSettingsViewModel _warmUpReportSettingsView;
        public SessionReportSettingsViewModel WarmUpReportSettingsView
        {
            get => _warmUpReportSettingsView;
            set => SetProperty(ref _warmUpReportSettingsView, value);
        }

        private int _minimumSessionLength;
        public int MinimumSessionLength
        {
            get => _minimumSessionLength;
            set => SetProperty(ref _minimumSessionLength, value);
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

        private static string ReplaceSpecialDirs(string path)
        {
            path = path.Replace(@"%MyDocuments%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            return path;
        }
    }
}