namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SettingsWindow
{
    using SecondMonitor.ViewModels;
    using Settings.DTO;

    public interface IGraphSettingsViewModel : IViewModel<GraphSettingsDto>
    {
        GraphLocationKind GraphLocation { get; set; }
    }
}