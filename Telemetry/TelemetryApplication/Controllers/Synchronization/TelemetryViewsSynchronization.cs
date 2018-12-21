namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public class TelemetryViewsSynchronization : ITelemetryViewsSynchronization
    {
        public event EventHandler<TelemetrySessionArgs> NewSessionLoaded;


        public void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto)
        {
            TelemetrySessionArgs args = new TelemetrySessionArgs(sessionInfoDto);
            NewSessionLoaded?.Invoke(this, args);
        }
    }
}