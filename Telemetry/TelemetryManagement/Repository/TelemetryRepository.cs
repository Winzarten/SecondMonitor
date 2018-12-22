namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.IO;
    using System.Linq;
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
            string fileName = Path.Combine(directory, $"{lapTelemetry.LapSummary.LapNumber}.xml");
            Logger.Info($"Saving lap info {lapTelemetry.LapSummary.LapNumber} to file: {fileName}");
            Directory.CreateDirectory(directory);
            Save(lapTelemetry, fileName);
        }

        public SessionInfoDto LoadSessionInformation(string sessionIdentifier)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));
            string directory = Path.Combine(_repositoryDirectory, sessionIdentifier);
            string fileName = Path.Combine(directory, SessionInfoFile);
            Logger.Info($"Loading Session info: {fileName}");

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
                 return xmlSerializer.Deserialize(file) as SessionInfoDto;
            }
        }

        public LapTelemetryDto LoadLapTelemetryDto(string sessionIdentifier, int lapNumber)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LapTelemetryDto));
            string directory = Path.Combine(_repositoryDirectory, sessionIdentifier);
            string fileName = Path.Combine(directory, $"{lapNumber}.xml");
            Logger.Info($"Loading lap info {lapNumber} from file: {fileName}");

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
                return xmlSerializer.Deserialize(file) as LapTelemetryDto;
            }
        }

        private void Save(SessionInfoDto sessionInfoDto, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                xmlSerializer.Serialize(file, sessionInfoDto);
            }

            RemoveObsoleteSessions();
        }

        private void Save(LapTelemetryDto lapTelemetryDto, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LapTelemetryDto));

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                xmlSerializer.Serialize(file, lapTelemetryDto);
            }
        }

        private void RemoveObsoleteSessions()
        {
            DirectoryInfo info = new DirectoryInfo(_repositoryDirectory);
            DirectoryInfo[] dis = info.GetDirectories().OrderBy(x => x.CreationTime).ToArray();
            if (dis.Length <= _maxStoredSessions)
            {
                return;
            }

            int toDelete = dis.Length - _maxStoredSessions;
            for (int i = 0; i < toDelete; i++)
            {
                Directory.Delete(dis[i].FullName, true);
            }
        }

    }
}
