﻿namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;

    using SecondMonitor.DataModel.Snapshot.Drivers;

    public class PendingSector
    {
        private static readonly TimeSpan MaxPendingTime = TimeSpan.FromSeconds(3);

        private readonly TimeSpan _initializationTime;

        public PendingSector(SectorTiming sector, TimeSpan initializationTime)
        {
            Sector = sector;
            Sector.SwitchToPending(initializationTime);
            _initializationTime = initializationTime;
        }

        public SectorTiming Sector { get; }

        public bool HasTimedOut(TimeSpan currentTime)
        {
            return (currentTime - _initializationTime) > MaxPendingTime;
        }

        public bool HasValidTimingInformation(DriverInfo driverInfo)
        {
            return SectorTiming.PickTimingFormDriverInfo(driverInfo, Sector.SectorNumber) > TimeSpan.Zero;
        }



    }
}