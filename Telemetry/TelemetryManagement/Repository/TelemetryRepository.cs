namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System;
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
        private readonly Dictionary<string, string> _sessionIdToDirectoryDictionary;

        public TelemetryRepository(string repositoryDirectory, int maxStoredSessions)
        {
            _repositoryDirectory = repositoryDirectory;
            _maxStoredSessions = maxStoredSessions;
            _sessionIdToDirectoryDictionary = new Dictionary<string, string>();
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
            dis.ForEach(x => sessions.Add(OpenRecentSession(x.Name)));
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

        public SessionInfoDto OpenRecentSession(string sessionIdentifier)
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir), sessionIdentifier);
            return OpenSession(directory);
        }

        public void CloseRecentSession(string sessionIdentifier)
        {
            _sessionIdToDirectoryDictionary.Remove(sessionIdentifier);
        }

        public LapTelemetryDto LoadLapTelemetryDtoFromAnySession(LapSummaryDto lapSummaryDto)
        {
            if (!_sessionIdToDirectoryDictionary.TryGetValue(lapSummaryDto.SessionIdentifier, out string directory))
            {
                throw new InvalidOperationException($"Session {lapSummaryDto.SessionIdentifier} is not opened. Unable to load lap {lapSummaryDto.Id}");
            }

            string fileName = Path.Combine(directory, $"{lapSummaryDto.LapNumber}{FileSuffix}");
            return LoadLapTelemetryDto(fileName);
        }

        private LapTelemetryDto LoadLapTelemetryDto(string fileName)
        {
            Logger.Info($"Loading from file: {fileName}");

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
                //return xmlSerializer.Deserialize(file) as LapTelemetryDto;
                BinaryFormatter bf = new BinaryFormatter();
                return (LapTelemetryDto)bf.Deserialize(file);
            }
        }

        private SessionInfoDto OpenSession(string sessionDirectory)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));
            string fileName = Path.Combine(sessionDirectory, SessionInfoFile);
            Logger.Info($"Loading Session info: {fileName}");
            SessionInfoDto sessionInfoDto;

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
               sessionInfoDto = (SessionInfoDto)xmlSerializer.Deserialize(file);
            }

            _sessionIdToDirectoryDictionary[sessionInfoDto.Id] = sessionDirectory;
            return sessionInfoDto;
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
