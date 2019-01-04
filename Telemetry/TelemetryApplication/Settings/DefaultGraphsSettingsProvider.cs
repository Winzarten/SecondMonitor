namespace SecondMonitor.Telemetry.TelemetryApplication.Settings
{
    using System.Linq;
    using ViewModels.GraphPanel;

    public class DefaultGraphsSettingsProvider : IGraphsSettingsProvider
    {
        private static readonly string[] LeftGraphs = new[] { "Speed", "Engine RPM", "Gear", "Lateral Acceleration", "Horizontal Acceleration" };
        private static readonly string[] RightGraphs = new[] { "Lap Time", "Throttle", "Brake", "Clutch" };

        public bool IsGraphViewModelLeft(IGraphViewModel graphViewModel)
        {
            return LeftGraphs.Contains(graphViewModel.Title);
        }

        public bool IsGraphViewModelRight(IGraphViewModel graphViewModel)
        {
            return RightGraphs.Contains(graphViewModel.Title);
        }
    }
}