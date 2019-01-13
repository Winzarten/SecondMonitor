namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using ViewModels.GraphPanel;

    public interface IGraphsSettingsProvider
    {
        bool IsGraphViewModelLeft(IGraphViewModel graphViewModel);
        bool IsGraphViewModelRight(IGraphViewModel graphViewModel);
        int GetGraphPriority(IGraphViewModel graphViewModel);
    }
}