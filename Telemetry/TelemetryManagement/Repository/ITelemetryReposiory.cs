namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using DTO;

    public interface ITelemetryRepository
    {
        IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions();
        IReadOnlyCollection<SessionInfoDto> GetAllArchivedSessions();
        IReadOnlyCollection<SessionInfoDto> LoadPreviouslyLoadedSessions(List<string> sessionIds);

        void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto OpenRecentSession(string sessionIdentifier);
        void CloseSession(string sessionIdentifier);
        LapTelemetryDto LoadLapTelemetryDtoFromAnySession(LapSummaryDto lapSummaryDto);
        LapTelemetryDto LoadLapTelemetryDto(FileInfo file);
        string GetLastRecentSessionIdentifier();

        Task ArchiveSessions(SessionInfoDto sessionInfoDto);

        Task OpenSessionFolder(SessionInfoDto sessionInfoDto);
        void DeleteSession(SessionInfoDto sessionInfoDto);
    }
}