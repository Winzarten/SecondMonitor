namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public interface IGraphViewSynchronization
    {
        event EventHandler<ScaleEventArgs> ScaleChanged;
        event EventHandler<PanEventArgs> PanChanged;

        void NotifyScaleChanged(object sender, double newScale);
        void NotifyPanChanged(object sender, double minimum, double maximum);
    }
}