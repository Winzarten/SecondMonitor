namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using DTO;

    public interface ITelemetryRepository
    {
        void SaveRecentSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveRecentSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto LoadRecentSessionInformation(string sessionIdentifier);
        LapTelemetryDto LoadRecentLapTelemetryDto(string sessionIdentifier, int lapNumber);
        string GetLastRecentSessionIdentifier();

    }
}