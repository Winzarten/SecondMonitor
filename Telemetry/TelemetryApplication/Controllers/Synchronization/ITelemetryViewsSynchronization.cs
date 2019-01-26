namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using DataModel.Telemetry;
    using TelemetryManagement.DTO;

    public interface ITelemetryViewsSynchronization
    {
        event EventHandler<EventArgs> LapLoadingStarted;
        event EventHandler<EventArgs> LapLoadingFinished;
        event EventHandler<TelemetrySessionArgs> NewSessionLoaded;
        event EventHandler<LapTelemetryArgs> LapLoaded;
        event EventHandler<LapSummaryArgs> LapUnloaded;
        event EventHandler<TelemetrySnapshotArgs> SyncTelemetryView;

        void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto);
        void NotifyLapLoaded(LapTelemetryDto lapTelemetryDto);
        void NotifyLapUnloaded(LapSummaryDto lapSummary);
        void NotifySynchronizeToSnapshot(TimedTelemetrySnapshot telemetrySnapshot, LapSummaryDto lapSummary);
        void NotifyLapLoadingStarted();
        void NotifyLapLoadingFinished();
    }
}