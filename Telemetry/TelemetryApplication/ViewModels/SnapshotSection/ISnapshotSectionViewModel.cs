namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SnapshotSection
{
    using System.Collections.ObjectModel;
    using DataModel.BasicProperties;
    using Replay;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.CarStatus;
    using TelemetryManagement.DTO;

    public interface ISnapshotSectionViewModel : IViewModel
    {
        ReadOnlyCollection<LapSummaryDto> AvailableLaps { get; }
        LapSummaryDto SelectedLap { get; set; }
        IReplayViewModel ReplayViewModel { get; }
        IPedalSectionViewModel PedalSectionViewModel { get; }
        CarWheelsViewModel CarWheelsViewModel { get; }
        TemperatureUnits TemperatureUnits { get; set; }
        PressureUnits PressureUnits { get; set; }



        void AddAvailableLap(LapSummaryDto lapSummaryDto);
        void RemoveAvailableLap(LapSummaryDto lapSummaryDto);
        void ClearAvailableLaps();

    }
}