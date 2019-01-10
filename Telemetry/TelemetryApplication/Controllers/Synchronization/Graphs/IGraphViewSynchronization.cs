namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public interface IGraphViewSynchronization
    {
        event EventHandler<PanEventArgs> PanChanged;
        event EventHandler<WheelVisibilityArgs> WheelVisibilityChanged;
        event EventHandler<TyreTempVisibilityArgs> TyreTempVisibilityChanged;

        void NotifyPanChanged(object sender, double minimum, double maximum);
        void NotifyWheelVisibilityChanged(object sender, bool frontLeftVisible, bool frontRightVisible, bool rearLeftVisible, bool rearRightVisible);
        void NotifyTyreTempVisibilityChanged(object sender, bool leftTempVisible, bool middleTempVisible, bool rightTempVisible, bool coreTempVisible);
    }
}