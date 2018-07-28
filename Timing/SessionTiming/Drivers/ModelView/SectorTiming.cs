namespace SecondMonitor.Timing.SessionTiming.Drivers.ModelView
{
    using System;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public class SectorTiming : IComparable
    {
        private readonly TimeSpan _startTime;

        public SectorTiming(int sectorNumber, SimulatorDataSet simulatorData, LapInfo lap)
        {
            SectorNumber = sectorNumber;
            _startTime = simulatorData.SessionInfo.SessionTime;
            Lap = lap;
        }

        public int SectorNumber { get; }

        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;

        public LapInfo Lap { get; }

        public void Tick(SimulatorDataSet dataSet, DriverInfo driverTiming)
        {
            Duration = dataSet.SessionInfo.SessionTime - _startTime;
        }

        public void Finish(DriverInfo driverTiming)
        {
            Duration = PickTiming(driverTiming);
            if (Duration.TotalSeconds <= 1)
            {
                Duration = TimeSpan.Zero;
            }
        }

        private TimeSpan PickTiming(DriverInfo driverInfo)
        {
            return PickTimingFormDriverInfo(driverInfo, SectorNumber);
        }

        public static TimeSpan PickTimingFormDriverInfo(DriverInfo driverInfo, int sectorNumber)
        {
            switch (sectorNumber)
            {
                case 1:
                    return driverInfo.Timing.LastSector1Time;
                case 2:
                    return driverInfo.Timing.LastSector2Time;
                case 3:
                    return driverInfo.Timing.LastSector3Time;
                default:
                    return TimeSpan.Zero;

            }
        }

        public static bool operator <(SectorTiming s1, SectorTiming s2)
        {
            return s1.Duration < s2.Duration;
        }

        public static bool operator >(SectorTiming s1, SectorTiming s2)
        {
            return s1.Duration > s2.Duration;
        }

        public static bool operator <=(SectorTiming s1, SectorTiming s2)
        {
            return s1.Duration <= s2.Duration;
        }

        public static bool operator >=(SectorTiming s1, SectorTiming s2)
        {
            return s1.Duration >= s2.Duration;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SectorTiming))
            {
                return -1;
            }
            return Duration.CompareTo(((SectorTiming)obj).Duration);
        }
    }
}