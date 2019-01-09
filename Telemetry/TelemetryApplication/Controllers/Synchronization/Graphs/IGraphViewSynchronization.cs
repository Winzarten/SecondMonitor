namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public interface IGraphViewSynchronization
    {
        event EventHandler<PanEventArgs> PanChanged;
        event EventHandler<WheelVisibilityArgs> WheelVisibilityChanged;

        void NotifyPanChanged(object sender, double minimum, double maximum);

        void NotifyWheelVisibilityChanged(object sender, bool frontLeftVisible, bool frontRightVisible, bool rearLeftVisible, bool rearRightVisible);
    }
}