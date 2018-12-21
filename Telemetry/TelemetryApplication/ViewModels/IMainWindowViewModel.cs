namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using LapPicker;
    using SecondMonitor.ViewModels;

    public interface IMainWindowViewModel : IAbstractViewModel
    {
        ILapSelectionViewModel LapSelectionViewModel { get; }
    }
}