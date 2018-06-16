namespace SecondMonitor.Timing.Settings
{
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using SecondMonitor.Timing.Settings.ModelView;

    public class DisplaySettingAutoSaver
    {
        private DisplaySettingsModelView _displaySettings;
        private bool _inSave;

        public DisplaySettingAutoSaver(string filePath) => FilePath = filePath;

        public string FilePath { get; private set; }

        public DisplaySettingsModelView DisplaySettingsModelView
        {
            get => _displaySettings;
            set
            {
                UnregisteredDisplaySettings();
                _displaySettings = value;
                RegisterDisplaySettings();
            }
        }

        private void UnregisteredDisplaySettings()
        {
            if (_displaySettings == null)
            {
                return;
            }

            _displaySettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            _displaySettings.PracticeSessionDisplayOptions.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            _displaySettings.QualificationSessionDisplayOptions.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            _displaySettings.RaceSessionDisplayOptions.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            UnRegisterColumnsProperties(_displaySettings.PracticeSessionDisplayOptions.ColumnsSettings);
            UnRegisterColumnsProperties(_displaySettings.QualificationSessionDisplayOptions.ColumnsSettings);
            UnRegisterColumnsProperties(_displaySettings.RaceSessionDisplayOptions.ColumnsSettings);
            UnRegisterReportingSettings(_displaySettings.ReportingSettings);
        }

        private void UnRegisterColumnsProperties(ColumnsSettingsModelView columnsSettingsModelView)
        {
            columnsSettingsModelView.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Position.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Name.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CarName.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CompletedLaps.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastLapTime.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Pace.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.BestLap.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CurrentLapProgressTime.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastPitInfo.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TimeToPlayer.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TopSpeed.PropertyChanged -= DisplaySettingsOnPropertyChanged;
        }

        private void UnRegisterReportingSettings(ReportingSettingsModelView reportingSettings)
        {
            reportingSettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            reportingSettings.PracticeReportSettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            reportingSettings.QualificationReportSetting.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            reportingSettings.WarmUpReportSettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
            reportingSettings.RaceReportSettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
        }

        private void RegisterColumnsProperties(ColumnsSettingsModelView columnsSettingsModelView)
        {
            columnsSettingsModelView.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Position.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Name.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CarName.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CompletedLaps.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastLapTime.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Pace.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.BestLap.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CurrentLapProgressTime.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastPitInfo.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TimeToPlayer.PropertyChanged += DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TopSpeed.PropertyChanged += DisplaySettingsOnPropertyChanged;
        }

        private void RegisterDisplaySettings()
        {
            if (_displaySettings == null)
            {
                return;
            }

            _displaySettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
            _displaySettings.PracticeSessionDisplayOptions.PropertyChanged += DisplaySettingsOnPropertyChanged;
            _displaySettings.QualificationSessionDisplayOptions.PropertyChanged += DisplaySettingsOnPropertyChanged;
            _displaySettings.RaceSessionDisplayOptions.PropertyChanged += DisplaySettingsOnPropertyChanged;
            RegisterColumnsProperties(_displaySettings.PracticeSessionDisplayOptions.ColumnsSettings);
            RegisterColumnsProperties(_displaySettings.QualificationSessionDisplayOptions.ColumnsSettings);
            RegisterColumnsProperties(_displaySettings.RaceSessionDisplayOptions.ColumnsSettings);
            RegisterReportingSettings(_displaySettings.ReportingSettings);
        }

        private void RegisterReportingSettings(ReportingSettingsModelView reportingSettings)
        {
            reportingSettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
            reportingSettings.PracticeReportSettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
            reportingSettings.QualificationReportSetting.PropertyChanged += DisplaySettingsOnPropertyChanged;
            reportingSettings.WarmUpReportSettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
            reportingSettings.RaceReportSettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
        }

        private void DisplaySettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
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
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_displaySettings.ToModel(), Formatting.Indented));
        }
    
    }
}
