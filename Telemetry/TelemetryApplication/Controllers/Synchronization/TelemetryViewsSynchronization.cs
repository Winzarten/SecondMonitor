﻿namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public class TelemetryViewsSynchronization : ITelemetryViewsSynchronization
    {
        public event EventHandler<TelemetrySessionArgs> NewSessionLoaded;
        public event EventHandler<LapTelemetryArgs> LapLoaded;
        public event EventHandler<LapSummaryArgs> LapUnloaded;
        public event EventHandler<TelemetrySnapshotArgs> SyncTelemetryView;

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

        public void NotifyLapUnloaded(LapSummaryDto lapSummaryDto)
        {
            LapSummaryArgs args = new LapSummaryArgs(lapSummaryDto);
            LapUnloaded?.Invoke(this, args);
        }

        public void NotifySynchronizeToSnapshot(TimedTelemetrySnapshot telemetrySnapshot, LapSummaryDto lapSummary)
        {
            TelemetrySnapshotArgs args = new TelemetrySnapshotArgs(telemetrySnapshot, lapSummary);
            SyncTelemetryView?.Invoke(this, args);
        }
    }
}