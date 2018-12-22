namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class TelemetrySnapshotArgs : EventArgs
    {
        public TelemetrySnapshotArgs(TimedTelemetrySnapshot telemetrySnapshot, LapSummaryDto lapSummaryDto)
        {
            TelemetrySnapshot = telemetrySnapshot;
            LapSummaryDto = lapSummaryDto;
        }

        public TimedTelemetrySnapshot TelemetrySnapshot { get; }
        public LapSummaryDto LapSummaryDto { get; }
    }
}