namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public interface ITelemetryViewsSynchronization
    {
        event EventHandler<TelemetrySessionArgs> NewSessionLoaded;
        event EventHandler<LapTelemetryArgs> LapLoaded;
        event EventHandler<LapNumberArgs> LapUnloaded;

        void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto);
        void NotifyLapLoaded(LapTelemetryDto lapTelemetryDto);
        void NotifyLapUnloaded(int lapNumber);
    }
}