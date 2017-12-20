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

        public DisplaySettingAutoSaver(string filePath) => this.FilePath = filePath;

        public string FilePath { get; private set; }

        public DisplaySettingsModelView DisplaySettingsModelView
        {
            get => this._displaySettings;
            set
            {
                this.UnregisteredDisplaySettings();
                this._displaySettings = value;
                this.RegisterDisplaySettings();
            }
        }

        private void UnregisteredDisplaySettings()
        {
            if (this._displaySettings == null)
            {
                return;
            }
            this._displaySettings.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.PracticeSessionDisplayOptions.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.QualificationSessionDisplayOptions.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.RaceSessionDisplayOptions.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            UnRegisterColumnsProperties(this._displaySettings.PracticeSessionDisplayOptions.ColumnsSettings);
            UnRegisterColumnsProperties(this._displaySettings.QualificationSessionDisplayOptions.ColumnsSettings);
            UnRegisterColumnsProperties(this._displaySettings.RaceSessionDisplayOptions.ColumnsSettings);
        }

        private void UnRegisterColumnsProperties(ColumnsSettingsModelView columnsSettingsModelView)
        {
            columnsSettingsModelView.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Position.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Name.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CarName.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CompletedLaps.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastLapTime.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Pace.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.BestLap.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CurrentLapProgressTime.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastPitInfo.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TimeToPlayer.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TopSpeed.PropertyChanged -= this.DisplaySettingsOnPropertyChanged;
        }

        private void RegisterColumnsProperties(ColumnsSettingsModelView columnsSettingsModelView)
        {
            columnsSettingsModelView.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Position.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Name.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CarName.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CompletedLaps.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastLapTime.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.Pace.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.BestLap.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.CurrentLapProgressTime.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.LastPitInfo.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TimeToPlayer.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            columnsSettingsModelView.TopSpeed.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
        }

        private void RegisterDisplaySettings()
        {
            if (this._displaySettings == null)
            {
                return;
            }
            this._displaySettings.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.PracticeSessionDisplayOptions.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.QualificationSessionDisplayOptions.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            this._displaySettings.RaceSessionDisplayOptions.PropertyChanged += this.DisplaySettingsOnPropertyChanged;
            RegisterColumnsProperties(this._displaySettings.PracticeSessionDisplayOptions.ColumnsSettings);
            RegisterColumnsProperties(this._displaySettings.QualificationSessionDisplayOptions.ColumnsSettings);
            RegisterColumnsProperties(this._displaySettings.RaceSessionDisplayOptions.ColumnsSettings);        
        }

        private void DisplaySettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.Save();
        }

        public async void Save()
        {
            if (this._inSave)
            {
                return;
            }
            this._inSave = true;
            await Task.Delay(1000);
            this._inSave = false;
            File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(this._displaySettings.ToModel(),Formatting.Indented));
        }
    
    }
}
