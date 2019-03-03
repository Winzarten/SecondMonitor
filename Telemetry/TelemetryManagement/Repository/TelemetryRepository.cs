namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using DataModel.Extensions;
    using DTO;
    using NLog;

    public class TelemetryRepository : ITelemetryRepository
    {
        private const string SessionInfoFile = "_Session.xml";
        private const string FileSuffix = ".Lap";
        private const string RecentDir = "Recent";
        private const string ArchiveDir = "Archive";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _repositoryDirectory;
        private readonly int _maxStoredSessions;
        private readonly ConcurrentDictionary<string, (string directory, bool isRecent)> _sessionIdToDirectoryDictionary;

        public TelemetryRepository(string repositoryDirectory, int maxStoredSessions)
        {
            _repositoryDirectory = repositoryDirectory;
            _maxStoredSessions = maxStoredSessions;
            _sessionIdToDirectoryDictionary = new ConcurrentDictionary<string, (string directory, bool isRecent)>();
        }

        public IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions()
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, RecentDir));
            return GetAllSessionsFromDirectory(new DirectoryInfo(directory), true);

        }

        public IReadOnlyCollection<SessionInfoDto> GetAllArchivedSessions()
        {
            string directory = Path.Combine(Path.Combine(_repositoryDirectory, ArchiveDir));
            return GetAllSessionsFromDirectory(new DirectoryInfo(directory), false);
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

        public async Task ArchiveSessions(SessionInfoDto sessionInfoDto)
        {
            if (!_sessionIdToDirectoryDictionary.TryGetValue(sessionInfoDto.Id, out (string directory, bool isRecent) entry))
            {
                throw new InvalidOperationException($"Session {sessionInfoDto.Id} is not opened. Cannot archive");
            }

            if (!entry.isRecent)
            {
                throw new InvalidOperationException($"Session {sessionInfoDto.Id} is not a recent session, cannot archive");
            }

            string archiveDir = Path.Combine(Path.Combine(_repositoryDirectory, ArchiveDir), sessionInfoDto.Id);

            if (Directory.Exists(archiveDir))
            {
                Directory.Delete(archiveDir, true);
            }

            Directory.CreateDirectory(archiveDir);

            DirectoryInfo startDirectory = new DirectoryInfo(entry.directory);

            //Creates all of the directories and sub-directories
            foreach (FileInfo file in startDirectory.EnumerateFiles())
            {
                using (FileStream sourceStream = file.OpenRead())
                {
                    using (FileStream destinationStream = File.Create(Path.Combine(archiveDir, file.Name)))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
        }

        public async Task OpenSessionFolder(SessionInfoDto sessionInfoDto)
        {
            if (!_sessionIdToDirectoryDictionary.TryGetValue(sessionInfoDto.Id, out (string directory, bool isRecent) entry))
            {
                throw new InvalidOperationException($"Session {sessionInfoDto.Id} is not opened. Cannot open folder");
            }

            await Task.Run(() => { Process.Start(entry.directory); });
        }

        public void DeleteSession(SessionInfoDto sessionInfoDto)
        {
            if (!_sessionIdToDirectoryDictionary.TryGetValue(sessionInfoDto.Id, out (string directory, bool isRecent) entry))
            {
                throw new InvalidOperationException($"Session {sessionInfoDto.Id} is not opened.");
            }
            CloseSession(sessionInfoDto.Id);
            Directory.Delete(entry.directory, true);
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
            return OpenSession(directory, true);
        }

        public void CloseSession(string sessionIdentifier)
        {
            _sessionIdToDirectoryDictionary.TryRemove(sessionIdentifier, out (string directory, bool isRecent) entry);
        }

        public LapTelemetryDto LoadLapTelemetryDtoFromAnySession(LapSummaryDto lapSummaryDto)
        {
            if (!_sessionIdToDirectoryDictionary.TryGetValue(lapSummaryDto.SessionIdentifier, out (string directory, bool isRecent) entry))
            {
                throw new InvalidOperationException($"Session {lapSummaryDto.SessionIdentifier} is not opened. Unable to load lap {lapSummaryDto.Id}");
            }

            string fileName = Path.Combine(entry.directory, $"{lapSummaryDto.LapNumber}{FileSuffix}");
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

        private SessionInfoDto OpenSession(string sessionDirectory, bool isRecent)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionInfoDto));
            string fileName = Path.Combine(sessionDirectory, SessionInfoFile);
            Logger.Info($"Loading Session info: {fileName}");
            SessionInfoDto sessionInfoDto;

            using (FileStream file = File.Open(fileName, FileMode.Open))
            {
               sessionInfoDto = (SessionInfoDto)xmlSerializer.Deserialize(file);
            }

            _sessionIdToDirectoryDictionary.TryAdd(sessionInfoDto.Id, (sessionDirectory, isRecent));
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

        private IReadOnlyCollection<SessionInfoDto> GetAllSessionsFromDirectory(DirectoryInfo directory, bool recent)
        {
            DirectoryInfo[] dis = directory.GetDirectories().OrderBy(x => x.CreationTime).ToArray();

            if (dis.Length == 0)
            {
                return Enumerable.Empty<SessionInfoDto>().ToList().AsReadOnly();
            }

            List<SessionInfoDto> sessions = new List<SessionInfoDto>();
            dis.ForEach(x => sessions.Add(OpenSession(x.FullName, recent)));
            return sessions.AsReadOnly();
        }
    }
}
