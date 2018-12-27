﻿namespace SecondMonitor.Telemetry.TelemetryManagement.Repository
{
    using DTO;

    public interface ITelemetryRepository
    {
        void SaveSessionInformation(SessionInfoDto sessionInfoDto, string sessionIdentifier);
        void SaveSessionLap(LapTelemetryDto lapTelemetry, string sessionIdentifier);

        SessionInfoDto LoadSessionInformation(string sessionIdentifier);
        LapTelemetryDto LoadLapTelemetryDto(string sessionIdentifier, int lapNumber);
        string GetLastSessionIdentifier();

    }
}