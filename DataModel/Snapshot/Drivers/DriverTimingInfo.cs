namespace SecondMonitor.DataModel.Snapshot.Drivers
{
    using System;

    public class DriverTimingInfo
    {
        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;

        public int CurrentSector { get; set; } = 0;

        public TimeSpan CurrentSectorTime { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector1Time { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector2Time { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector3Time { get; set; } = TimeSpan.Zero;
    }
}
