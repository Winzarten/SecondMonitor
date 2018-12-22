namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using LapPicker;
    using SecondMonitor.ViewModels;
    using SnapshotSection;

    public interface IMainWindowViewModel : IAbstractViewModel
    {
        ILapSelectionViewModel LapSelectionViewModel { get; }
        ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
    }
}