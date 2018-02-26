namespace SecondMonitor.DataModel.Summary
{
    using System;

    public class Lap
    {
        public Lap(Driver driver)
        {
            Driver = driver;
        }

        public Driver Driver { get; set; }

        public int LapNumber { get; set; }

        public TimeSpan LapTime { get; set; }

        public TimeSpan Sector1 { get; set; }

        public TimeSpan Sector2 { get; set; }

        public TimeSpan Sector3 { get; set; }

    }
}