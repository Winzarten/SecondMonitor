namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization.Graphs
{
    using System;

    public class TyreTempVisibilityArgs : EventArgs
    {
        public TyreTempVisibilityArgs(bool leftTempVisible, bool middleTempVisible, bool rightTempVisible, bool coreTempVisible)
        {
            LeftTempVisible = leftTempVisible;
            MiddleTempVisible = middleTempVisible;
            RightTempVisible = rightTempVisible;
            CoreTempVisible = coreTempVisible;
        }

        public bool LeftTempVisible { get; }
        public bool MiddleTempVisible { get; }
        public bool RightTempVisible { get; }
        public bool CoreTempVisible { get; }
    }
}