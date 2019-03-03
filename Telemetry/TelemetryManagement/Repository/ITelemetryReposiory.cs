namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DTO;

    public interface ITelemetryRepository
    {
        IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions();
        IReadOnlyCollection<SessionInfoDto> GetAllArchivedSessions();

        void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto OpenRecentSession(string sessionIdentifier);
        void CloseSession(string sessionIdentifier);
        LapTelemetryDto LoadLapTelemetryDtoFromAnySession(LapSummaryDto lapSummaryDto);
        string GetLastRecentSessionIdentifier();

        Task ArchiveSessions(SessionInfoDto sessionInfoDto);

        Task OpenSessionFolder(SessionInfoDto sessionInfoDto);
        void DeleteSession(SessionInfoDto sessionInfoDto);
    }
}