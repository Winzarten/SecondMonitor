namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.IO;
    using System.Xml.Serialization;
    using DTO;
    using NLog;

    public class TelemetryRepository : ITelemetryRepository
    {
        private const string SessionInfoFile = "_Session.xml";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _repositoryDirectory;
        private readonly int _maxStoredSessions;

        public TelemetryRepository(string repositoryDirectory, int maxStoredSessions)
        {
            _repositoryDirectory = repositoryDirectory;
            _maxStoredSessions = maxStoredSessions;
        }

        public void SaveSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier)
        {
            string directory = Path.Combine(_repositoryDirectory, sessionIdentifier);
            string fileName = Path.Combine(directory, SessionInfoFile);
            Logger.Info($"Saving session info to file: {fileName}");
            Directory.CreateDirectory(directory);
            Save(sessionInfoDto, fileName);
        }

        public void SaveSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier)
        {
            string directory = Path.Combine(_repositoryDirectory, sessionIdentifier);
            string fileName = Path.Combine(directory, $"{lapTelemetry.LapNumber}.xml");
            Logger.Info($"Saving lap info {lapTelemetry.LapNumber} to file: {fileName}");
            Directory.CreateDirectory(directory);
            Save(lapTelemetry, fileName);
        }

        private void Save(SessionInfoDto sessionInfoDto, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                xmlSerializer.Serialize(file, sessionInfoDto);
            }
        }

        private void Save(LapTelemetryDto lapTelemetryDto, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LapTelemetryDto));

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                xmlSerializer.Serialize(file, lapTelemetryDto);
            }
        }

    }
}
