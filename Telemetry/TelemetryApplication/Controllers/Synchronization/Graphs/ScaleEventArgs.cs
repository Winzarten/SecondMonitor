namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class ScaleEventArgs : EventArgs
    {
        public double NewScale { get; }

        public ScaleEventArgs(double newScale)
        {
            NewScale = newScale;
        }
    }
}