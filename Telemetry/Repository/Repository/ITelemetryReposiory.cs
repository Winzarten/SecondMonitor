namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using DTO;

    public interface ITelemetryRepository
    {
        void SaveSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);
    }
}