namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using LapPicker;

    public interface IMainWindowViewModel
    {
        ILapSelectionViewModel LapSelectionViewModel { get; }
    }
}