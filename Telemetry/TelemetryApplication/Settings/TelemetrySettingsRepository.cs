namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using DTO;
    using NLog;

    public class TelemetrySettingsRepository : ITelemetrySettingsRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string SettingsFileName = "Config.xml";
        private readonly string _repositoryDirectory;
        private readonly XmlSerializer _xmlSerializer;

        public TelemetrySettingsRepository(ISettingsProvider settingsProvider)
        {
            _repositoryDirectory = settingsProvider.TelemetryRepositoryPath;
            _xmlSerializer = new XmlSerializer(typeof(TelemetrySettingsDto));
        }

        public bool TryLoadTelemetrySettings(out TelemetrySettingsDto telemetrySettings)
        {
            string fullPathName = Path.Combine(_repositoryDirectory, SettingsFileName);
            try
            {
                using (TextReader file = File.OpenText(fullPathName))
                {
                    XmlReader reader = XmlReader.Create(file, new XmlReaderSettings() { CheckCharacters = false });
                    telemetrySettings = (TelemetrySettingsDto)_xmlSerializer.Deserialize(reader);
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
        }
    }
}