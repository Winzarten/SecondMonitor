namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using DTO;
    using NLog;
    using SecondMonitor.ViewModels.Settings;

    public class TelemetrySettingsRepository : ITelemetrySettingsRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string SettingsFileName = "Config.xml";
        private readonly string _repositoryDirectory;
        private readonly XmlSerializer _xmlSerializer;
        private TelemetrySettingsDto _cachedSettings;

        public TelemetrySettingsRepository(ISettingsProvider settingsProvider)
        {
            _repositoryDirectory = settingsProvider.TelemetryRepositoryPath;
            _xmlSerializer = new XmlSerializer(typeof(TelemetrySettingsDto));
        }

        public event EventHandler<EventArgs> SettingsChanged;

        public TelemetrySettingsDto LoadOrCreateNew()
        {
            if (TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettingsDto))
            {
                return telemetrySettingsDto;
            }

            return new TelemetrySettingsDto()
            {
                GraphSettings = new List<GraphSettingsDto>()
            };
        }

        public bool TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings)
        {
            if (_cachedSettings != null)
            {
                telemetrySettings = _cachedSettings;
                return true;
            }

            string fullPathName = Path.Combine(_repositoryDirectory, SettingsFileName);
            try
            {
                using (TextReader file = File.OpenText(fullPathName))
                {
                    XmlReader reader = XmlReader.Create(file, new XmlReaderSettings() { CheckCharacters = false });
                    telemetrySettings = (TelemetrySettingsDto)_xmlSerializer.Deserialize(reader);
                    _cachedSettings = telemetrySettings;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Unable to load telemetry settings");
                if (File.Exists(fullPathName))
                {
                    File.Delete(fullPathName);
                }

                telemetrySettings = null;
                return false;
            }
        }

        public void SaveTelemetrySettings(TelemetrySettingsDto telemetrySettings)
        {
            string fullPathName = Path.Combine(_repositoryDirectory, SettingsFileName);
            Directory.CreateDirectory(_repositoryDirectory);
            using (FileStream file = File.Exists(fullPathName) ? File.Open(fullPathName, FileMode.Truncate) : File.Create(fullPathName))
            {
                _xmlSerializer.Serialize(file, telemetrySettings);
            }

            _cachedSettings = telemetrySettings;
            SettingsChanged?.Invoke(this, new EventArgs());
        }
    }
}