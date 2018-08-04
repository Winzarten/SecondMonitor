using SecondMonitor.DataModel.Telemetry;

namespace SecondMonitor.DataModel.Summary
{
    using System;

    public class Lap
    {
        public Lap(Driver driver, bool isValid)
        {
            Driver = driver;
            IsValid = isValid;
        }

        public Driver Driver { get; set; }

        public int LapNumber { get; set; }

        public bool IsValid { get; set; }

        public TimeSpan LapTime { get; set; } = TimeSpan.Zero;

        public TimeSpan Sector1 { get; set; } = TimeSpan.Zero;

        public TimeSpan Sector2 { get; set; } = TimeSpan.Zero;

        public TimeSpan Sector3 { get; set; } = TimeSpan.Zero;

        public TelemetrySnapshot LapEndSnapshot { get; set; }

    }
}