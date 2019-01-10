namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class GraphViewSynchronization : IGraphViewSynchronization
    {
        public event EventHandler<PanEventArgs> PanChanged;
        public event EventHandler<WheelVisibilityArgs> WheelVisibilityChanged;
        public event EventHandler<TyreTempVisibilityArgs> TyreTempVisibilityChanged;

        public void NotifyPanChanged(object sender, double minimum, double maximum)
        {
            PanChanged?.Invoke(sender, new PanEventArgs(minimum, maximum));
        }

        public void NotifyWheelVisibilityChanged(object sender, bool frontLeftVisible, bool frontRightVisible, bool rearLeftVisible, bool rearRightVisible)
        {
            WheelVisibilityChanged?.Invoke(sender, new WheelVisibilityArgs(frontLeftVisible, frontRightVisible, rearLeftVisible, rearRightVisible));
        }

        public void NotifyTyreTempVisibilityChanged(object sender, bool leftTempVisible, bool middleTempVisible, bool rightTempVisible, bool coreTempVisible)
        {
            TyreTempVisibilityChanged?.Invoke(sender, new TyreTempVisibilityArgs(leftTempVisible, middleTempVisible, rightTempVisible, coreTempVisible));
        }
    }
}