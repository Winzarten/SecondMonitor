namespace SecondMonitor.Timing.Settings
{
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using ViewModel;

    public class DisplaySettingAutoSaver
    {
        private DisplaySettingsViewModel _displaySettingsView;
        private bool _inSave;

        public DisplaySettingAutoSaver(string filePath) => FilePath = filePath;

        public string FilePath { get; private set; }

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsView;
            set
            {
                UnregisteredDisplaySettings();
                _displaySettingsView = value;
                RegisterDisplaySettings();
            }
        }

        private void UnregisteredDisplaySettings()
        {
            if (_displaySettingsView == null)
            {
                return;
            }

            _displaySettingsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.PracticeSessionDisplayOptionsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.QualificationSessionDisplayOptionsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.RaceSessionDisplayOptionsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.MapDisplaySettingsViewModel.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            UnRegisterColumnsProperties(_displaySettingsView.PracticeSessionDisplayOptionsView.ColumnsSettingsView);
            UnRegisterColumnsProperties(_displaySettingsView.QualificationSessionDisplayOptionsView.ColumnsSettingsView);
            UnRegisterColumnsProperties(_displaySettingsView.RaceSessionDisplayOptionsView.ColumnsSettingsView);
            UnRegisterReportingSettings(_displaySettingsView.ReportingSettingsView);
        }

        private void UnRegisterColumnsProperties(ColumnsSettingsViewModel columnsSettingsViewModel)
        {
            columnsSettingsViewModel.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Position.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Name.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CarName.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CompletedLaps.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.LastLapTime.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Pace.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.BestLap.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CurrentLapProgressTime.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.LastPitInfo.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.TimeToPlayer.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.TopSpeed.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
        }

        private void UnRegisterReportingSettings(ReportingSettingsViewModel reportingSettingsView)
        {
            reportingSettingsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.PracticeReportSettingsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.QualificationReportSettingView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.WarmUpReportSettingsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.RaceReportSettingsView.PropertyChanged -= DisplaySettingsViewOnPropertyChanged;
        }

        private void RegisterColumnsProperties(ColumnsSettingsViewModel columnsSettingsViewModel)
        {
            columnsSettingsViewModel.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Position.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Name.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CarName.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CompletedLaps.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.LastLapTime.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.Pace.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.BestLap.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.CurrentLapProgressTime.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.LastPitInfo.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.TimeToPlayer.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            columnsSettingsViewModel.TopSpeed.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
        }

        private void RegisterDisplaySettings()
        {
            if (_displaySettingsView == null)
            {
                return;
            }

            _displaySettingsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.PracticeSessionDisplayOptionsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.QualificationSessionDisplayOptionsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.RaceSessionDisplayOptionsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            _displaySettingsView.MapDisplaySettingsViewModel.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            RegisterColumnsProperties(_displaySettingsView.PracticeSessionDisplayOptionsView.ColumnsSettingsView);
            RegisterColumnsProperties(_displaySettingsView.QualificationSessionDisplayOptionsView.ColumnsSettingsView);
            RegisterColumnsProperties(_displaySettingsView.RaceSessionDisplayOptionsView.ColumnsSettingsView);
            RegisterReportingSettings(_displaySettingsView.ReportingSettingsView);
        }

        private void RegisterReportingSettings(ReportingSettingsViewModel reportingSettingsView)
        {
            reportingSettingsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.PracticeReportSettingsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.QualificationReportSettingView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.WarmUpReportSettingsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
            reportingSettingsView.RaceReportSettingsView.PropertyChanged += DisplaySettingsViewOnPropertyChanged;
        }

        private void DisplaySettingsViewOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Save();
        }

        public async void Save()
        {
            if (_inSave)
            {
                return;
            }

            _inSave = true;
            await Task.Delay(1000);
            _inSave = false;
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_displaySettingsView.ToModel(), Formatting.Indented));
        }

    }
}
