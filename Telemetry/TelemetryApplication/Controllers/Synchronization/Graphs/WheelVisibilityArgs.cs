namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class WheelVisibilityArgs : EventArgs
    {
        public WheelVisibilityArgs(bool frontLeftVisible, bool frontRightVisible, bool rearLeftVisible, bool rearRightVisible)
        {
            FrontLeftVisible = frontLeftVisible;
            FrontRightVisible = frontRightVisible;
            RearLeftVisible = rearLeftVisible;
            RearRightVisible = rearRightVisible;
        }

        public bool FrontLeftVisible { get; }
        public bool FrontRightVisible { get; }
        public bool RearLeftVisible { get; }
        public bool RearRightVisible { get; }
    }
}