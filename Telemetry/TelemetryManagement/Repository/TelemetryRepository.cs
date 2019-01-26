namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;
    using DataModel.Extensions;
    using DTO;
    using NLog;

    public class TelemetryRepository : ITelemetryRepository
    {
        private const string SessionInfoFile = "_Session.xml";
        private const string FileSuffix = ".Lap";
        private const string RecentDir = "Recent";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _repositoryDirectory;
        private readonly int _maxStoredSessions;

        public TelemetryRepository(string repositoryDirectory, int maxStoredSessions)
        {
            _repositoryDirectory = repositoryDirectory;
            _maxStoredSessions = maxStoredSessions;
        }

        public IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions()
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir));
            DirectoryInfo info = new DirectoryInfo(directory);
            DirectoryInfo[] dis = info.GetDirectories().OrderBy(x => x.CreationTime).ToArray();

            if (dis.Length == 0)
            {
                return Enumerable.Empty<SessionInfoDto>().ToList().AsReadOnly();
            }

            List<SessionInfoDto> sessions = new List<SessionInfoDto>();
            dis.ForEach(x => sessions.Add(LoadRecentSessionInformation(x.Name)));
            return sessions.AsReadOnly();

        }

        public void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier)
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir), sessionIdentifier);
            string fileName = Path.Combine(directory, SessionInfoFile);
            Logger.Info($"Saving session info to file: {fileName}");
            Directory.CreateDirectory(directory);
            Save(sessionInfoDto, fileName);
        }

        public string GetLastRecentSessionIdentifier()
        {
            string directory = Path.Combine(_repositoryDirectory, RecentDir);
            if (!Directory.Exists(directory))
            {
                return string.Empty;
            }
            Directory.CreateDirectory(directory);
            DirectoryInfo info = new DirectoryInfo(directory);
            DirectoryInfo[] dis = info.GetDirectories().OrderBy(x => x.CreationTime).ToArray();
            return dis.Last().Name;
        }

        public void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier)
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir), sessionIdentifier);
            string fileName = Path.Combine(directory, $"{lapTelemetry.LapSummary.LapNumber}{FileSuffix}");
            Logger.Info($"Saving lap info {lapTelemetry.LapSummary.LapNumber} to file: {fileName}");
            Directory.CreateDirectory(directory);
            Save(lapTelemetry, fileName);
        }

        public SessionInfoDto LoadRecentSessionInformation(string sessionIdentifier)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir), sessionIdentifier);
            string fileName = Path.Combine(directory, SessionInfoFile);
            Logger.Info($"Loading Session info: {fileName}");

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
                 return xmlSerializer.Deserialize(file) as SessionInfoDto;
            }
        }

        public LapTelemetryDto LoadRecentLapTelemetryDto(string sessionIdentifier, int lapNumber)
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir), sessionIdentifier);
            string fileName = Path.Combine(directory, $"{lapNumber}{FileSuffix}");
            Logger.Info($"Loading lap info {lapNumber} from file: {fileName}");

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
                //return xmlSerializer.Deserialize(file) as LapTelemetryDto;
                BinaryFormatter bf = new BinaryFormatter();
                return (LapTelemetryDto) bf.Deserialize(file);
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
            /*XmlSerializer xmlSerializer = new XmlSerializer(typeof(LapTelemetryDto));

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                xmlSerializer.Serialize(file, lapTelemetryDto);
            }*/

            using (FileStream file = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, lapTelemetryDto);
            }
        }

        private void RemoveObsoleteSessions()
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(_repositoryDirectory, RecentDir));
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
