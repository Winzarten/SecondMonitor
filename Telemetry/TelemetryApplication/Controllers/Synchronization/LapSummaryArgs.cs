namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using TelemetryManagement.DTO;

    public class LapSummaryArgs : EventArgs
    {
        public LapSummaryArgs(LapSummaryDto lapSummary)
        {
            LapSummary = lapSummary;
        }

        public LapSummaryDto LapSummary { get; }
    }
}