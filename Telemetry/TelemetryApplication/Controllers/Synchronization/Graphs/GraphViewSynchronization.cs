namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class GraphViewSynchronization : IGraphViewSynchronization
    {
        public event EventHandler<PanEventArgs> PanChanged;
        public event EventHandler<WheelVisibilityArgs> WheelVisibilityChanged;

        public void NotifyPanChanged(object sender, double minimum, double maximum)
        {
            PanChanged?.Invoke(sender, new PanEventArgs(minimum, maximum));
        }

        public void NotifyWheelVisibilityChanged(object sender, bool frontLeftVisible, bool frontRightVisible, bool rearLeftVisible, bool rearRightVisible)
        {
            WheelVisibilityChanged?.Invoke(sender, new WheelVisibilityArgs(frontLeftVisible, frontRightVisible, rearLeftVisible, rearRightVisible));
        }
    }
}