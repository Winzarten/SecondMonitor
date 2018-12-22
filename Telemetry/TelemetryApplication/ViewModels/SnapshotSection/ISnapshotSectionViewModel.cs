namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SnapshotSection
{
    using System.Collections.ObjectModel;
    using Replay;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface ISnapshotSectionViewModel : IAbstractViewModel
    {
        ReadOnlyCollection<LapSummaryDto> AvailableLaps { get; }
        LapSummaryDto SelectedLap { get; set; }
        IReplayViewModel ReplayViewModel { get; }

        void AddAvailableLap(LapSummaryDto lapSummaryDto);
        void RemoveAvailableLap(LapSummaryDto lapSummaryDto);
        void ClearAvailableLaps();

    }
}