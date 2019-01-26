namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using System.Collections.Generic;
    using DTO;

    public interface ITelemetryRepository
    {

        IReadOnlyCollection<SessionInfoDto> GetAllRecentSessions();
        void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto LoadRecentSessionInformation(string sessionIdentifier);
        LapTelemetryDto LoadRecentLapTelemetryDto(string sessionIdentifier, int lapNumber);
        string GetLastRecentSessionIdentifier();

    }
}