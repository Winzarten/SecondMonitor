namespace SecondMonitor.DataModel.Snapshot.Drivers
{
    using System;

    [Serializable]
    public sealed class DriverTimingInfo
    {
        public DriverTimingInfo()
        {
            
        }

        /// <summary>
        ///How far ahead the car in front of this driver is
        /// </summary>
        public TimeSpan GapAhead { get; set; }

        /// <summary>
        ///  How far behind the car behind of this driver is
        /// </summary>
        public TimeSpan GapBehind { get; set; }

        /// <summary>
        /// Gap to Player
        /// </summary>
        public TimeSpan GapToPlayer { get; set; }

        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;

        public TimeSpan CurrentLapTime { get; set; } = TimeSpan.Zero;

        public int CurrentSector { get; set; } = 0;

        public TimeSpan CurrentSectorTime { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector1Time { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector2Time { get; set; } = TimeSpan.Zero;

        public TimeSpan LastSector3Time { get; set; } = TimeSpan.Zero;
    }
}
