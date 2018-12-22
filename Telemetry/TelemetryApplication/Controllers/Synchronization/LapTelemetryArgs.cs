namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public class LapTelemetryArgs : EventArgs
    {
        public LapTelemetryArgs(LapTelemetryDto lapTelemetry)
        {
            LapTelemetry = lapTelemetry;
        }

        public LapTelemetryDto LapTelemetry { get; }
    }
}