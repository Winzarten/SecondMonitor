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
        event EventHandler<TelemetrySessionArgs> SessionAdded;
        event EventHandler<LapSummaryArgs> LapAddedToSession;
        event EventHandler<LapTelemetryArgs> LapLoaded;
        event EventHandler<LapSummaryArgs> LapUnloaded;
        event EventHandler<TelemetrySnapshotArgs> SyncTelemetryView;
        event EventHandler<LapSummaryArgs> ReferenceLapSelected;

        void NotifyNewSessionLoaded(SessionInfoDto sessionInfoDto);
        void NotifySessionAdded(SessionInfoDto sessionInfoDto);
        void NotifyLappAddedToSession(LapSummaryDto lapSummaryDto);
        void NotifyLapLoaded(LapTelemetryDto lapTelemetryDto);
        void NotifyLapUnloaded(LapSummaryDto lapSummary);
        void NotifySynchronizeToSnapshot(TimedTelemetrySnapshot telemetrySnapshot, LapSummaryDto lapSummary);
        void NotifyReferenceLapSelected(LapSummaryDto referenceLap);
        void NotifyLapLoadingStarted();
        void NotifyLapLoadingFinished();
    }
}