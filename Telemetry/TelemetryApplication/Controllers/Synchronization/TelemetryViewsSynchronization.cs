namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public class TelemetryViewsSynchronization : ITelemetryViewsSynchronization
    {
        public event EventHandler<TelemetrySessionArgs> NewSessionLoaded;
        public event EventHandler<LapTelemetryArgs> LapLoaded;
        public event EventHandler<LapNumberArgs> LapUnloaded;

        public void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto)
        {
            TelemetrySessionArgs args = new TelemetrySessionArgs(sessionInfoDto);
            NewSessionLoaded?.Invoke(this, args);
        }

        public void NotifyLapLoaded(LapTelemetryDto lapTelemetryDto)
        {
            LapTelemetryArgs lapTelemetryArgs = new LapTelemetryArgs(lapTelemetryDto);
            LapLoaded?.Invoke(this, lapTelemetryArgs);
        }

        public void NotifyLapUnloaded(int lapNumber)
        {
            LapNumberArgs args = new LapNumberArgs(lapNumber);
            LapUnloaded?.Invoke(this, args);
        }
    }
}