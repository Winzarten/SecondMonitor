namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.Collections.Generic;
    using DTO;

    public interface ITelemetryRepository
    {

        IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions();
        void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto OpenRecentSession(string sessionIdentifier);
        void CloseRecentSession(string sessionIdentifier);
        LapTelemetryDto LoadLapTelemetryDtoFromAnySession(LapSummaryDto lapSummaryDto);
        string GetLastRecentSessionIdentifier();

    }
}