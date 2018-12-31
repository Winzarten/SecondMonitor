namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using LapPicker;
    using MapView;
    using SecondMonitor.ViewModels;
    using SnapshotSection;

    public interface IMainWindowViewModel : IAbstractViewModel
    {
        bool IsBusy { get; set; }
        ILapSelectionViewModel LapSelectionViewModel { get; }
        ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
        IMapViewViewModel MapViewViewModel { get; }
    }
}