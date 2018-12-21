namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public interface ITelemetryViewsSynchronization
    {
        event EventHandler<TelemetrySessionArgs> NewSessionLoaded;

        void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto);
    }
}