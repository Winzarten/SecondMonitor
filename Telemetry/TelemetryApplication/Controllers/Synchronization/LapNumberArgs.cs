namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;

    public class LapNumberArgs : EventArgs
    {
        public int LapNumber { get; }

        public LapNumberArgs(int lapNumber)
        {
            LapNumber = lapNumber;
        }
    }
}