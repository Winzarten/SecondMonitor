namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class GraphViewSynchronization : IGraphViewSynchronization
    {
        public event EventHandler<ScaleEventArgs> ScaleChanged;
        public event EventHandler<PanEventArgs> PanChanged;

        public void NotifyScaleChanged(object sender, double newScale)
        {
            ScaleChanged?.Invoke(sender, new ScaleEventArgs(newScale));
        }

        public void NotifyPanChanged(object sender, double minimum, double maximum)
        {
            PanChanged?.Invoke(sender, new PanEventArgs(minimum, maximum));
        }
    }
}