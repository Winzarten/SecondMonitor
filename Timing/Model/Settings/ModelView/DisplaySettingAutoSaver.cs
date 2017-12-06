using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SecondMonitor.Timing.Model.Settings.ModelView
{
    public class DisplaySettingAutoSaver
    {
        private DisplaySettingsModelView _displaySettings;

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
                return;
            _displaySettings.PropertyChanged -= DisplaySettingsOnPropertyChanged;
        }

        private void RegisterDisplaySettings()
        {
            if (_displaySettings == null)
                return;
            _displaySettings.PropertyChanged += DisplaySettingsOnPropertyChanged;
        }

        private void DisplaySettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Save();
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_displaySettings.ToModel()));
        }
    
    }
}
