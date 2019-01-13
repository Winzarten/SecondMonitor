namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using ViewModels.GraphPanel;

    public interface IGraphsSettingsProvider
    {
        void ForceSettingsReload();
        bool IsGraphViewModelLeft(IGraphViewModel graphViewModel);
        bool IsGraphViewModelRight(IGraphViewModel graphViewModel);
        int GetGraphPriority(IGraphViewModel graphViewModel);
    }
}