namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public class TelemetrySessionArgs : EventArgs
    {
        public TelemetrySessionArgs(SessionInfoDto sessionInfoDto)
        {
            SessionInfoDto = sessionInfoDto;
        }

        public SessionInfoDto SessionInfoDto { get; }
    }
}