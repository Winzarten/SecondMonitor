namespace SecondMonitor.Timing.SessionTiming.Drivers.ModelView
{
    using System;

    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public class LapInfo
    {

        public class SectorCompletedArgs : EventArgs
        {
            public SectorCompletedArgs(SectorTiming sectorTiming)
            {
                SectorTiming = sectorTiming;
            }

            public SectorTiming SectorTiming { get; }

        }

        private bool _valid;

        private bool _completed;

        public LapInfo(TimeSpan startSessionTime, int lapNumber, DriverTiming driver, LapInfo previousLapInfo)
        {
            Driver = driver;
            LapStart = startSessionTime;
            LapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = false;
            PitLap = false;
            PreviousLap = previousLapInfo;
        }

        public LapInfo(TimeSpan startSessionTime, int lapNumber, DriverTiming driver, bool firstLap, LapInfo previousLapInfo)
        {
            Driver = driver;
            LapStart = startSessionTime;
            LapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = firstLap;
            PitLap = false;
            PreviousLap = previousLapInfo;
        }

        public event EventHandler<SectorCompletedArgs> SectorCompletedEvent;

        public event EventHandler<DriverTiming.LapEventArgs> LapInvalidatedEvent;
        public event EventHandler<DriverTiming.LapEventArgs> LapCompletedEvent;

        public TimeSpan LapStart { get; }

        public int LapNumber { get; private set; }

        public bool Valid
        {
            get => _valid;
            set
            {
                if (Valid && !value)
                {
                    OnLapInvalidatedEvent(new DriverTiming.LapEventArgs(this));
                }
                _valid = value;

            }
    }

        public DriverTiming Driver { get; }

        public bool FirstLap { get; }

        public bool InvalidBySim { get; set; }

        public bool PitLap { get; set; }

        public bool Completed
        {
            get => _completed;
            private set
            {
                _completed = value;
                if (Completed)
                {
                    OnLapCompletedEvent(new DriverTiming.LapEventArgs(this));
                }
            }

        }


        public LapInfo PreviousLap { get; }

        public SectorTiming Sector1 { get; private set; }

        public SectorTiming Sector2 { get; private set; }

        public SectorTiming Sector3 { get; private set; }

        public SectorTiming CurrentSector { get; private set; }

        public void FinishLap(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation)
            {
                LapEnd = dataSet.SessionInfo.SessionTime;
            }
            else
            {
                LapEnd = LapStart.Add(driverInfo.Timing.LastLapTime);
            }

            LapTime = LapEnd.Subtract(LapStart);
            Completed = true;
        }

        public void Tick(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            LapProgressTime = dataSet.SessionInfo.SessionTime.Subtract(LapStart);

            // Let 5 seconds for the source data noise, when lap count might not be properly updated at instance creation
            if (LapProgressTime.TotalSeconds < 5 && LapNumber != driverInfo.CompletedLaps + 1)
            {
                LapNumber = driverInfo.CompletedLaps + 1;
            }
            if (dataSet.SimulatorSourceInfo.SectorTimingSupport != DataInputSupport.NONE)
            {
                TickSectors(dataSet, driverInfo);
            }
        }

        private void TickSectors(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if ((dataSet.SimulatorSourceInfo.SectorTimingSupport == DataInputSupport.PLAYER_ONLY && !driverInfo.IsPlayer) || !Valid)
            {
                return;
            }

            if (CurrentSector == null && driverInfo.Timing.CurrentSector != 1)
            {
                return;
            }
            if (CurrentSector == null && driverInfo.Timing.CurrentSector == 1)
            {
                Sector1 = new SectorTiming(1, dataSet, this);
                CurrentSector = Sector1;
                return;
            }
            if (driverInfo.Timing.CurrentSector == 0)
            {
                return;
            }
            SectorTiming shouldBeActive = PickSector(driverInfo.Timing.CurrentSector);
            if (shouldBeActive == CurrentSector)
            {
                CurrentSector.Tick(dataSet, driverInfo);
                return;
            }
            CurrentSector.Finish(driverInfo);
            if (CurrentSector.Duration == TimeSpan.Zero)
            {
                Valid = false;
            }
            OnSectorCompleted(new SectorCompletedArgs(CurrentSector));
            switch (driverInfo.Timing.CurrentSector)
            {
                case 2:
                    Sector2 = new SectorTiming(2, dataSet, this);
                    CurrentSector = Sector2;
                    return;
                case 3:
                    Sector3 = new SectorTiming(3, dataSet, this);
                    CurrentSector = Sector3;
                    return;
            }
        }

        private SectorTiming PickSector(int sectorNumber)
        {
            switch (sectorNumber)
            {
                case 1:
                    return Sector1;
                case 2:
                    return Sector2;
                case 3:
                    return Sector3;
                default:
                    return null;
            }
        }

        public TimeSpan LapEnd { get; private set; }

        public TimeSpan LapTime { get; private set; } = TimeSpan.Zero;

        public TimeSpan LapProgressTime { get; private set; }

        protected virtual void OnSectorCompleted(SectorCompletedArgs e)
        {
            SectorCompletedEvent?.Invoke(this, e);
        }

        protected virtual void OnLapInvalidatedEvent(DriverTiming.LapEventArgs e)
        {
            LapInvalidatedEvent?.Invoke(this, e);
        }

        protected virtual void OnLapCompletedEvent(DriverTiming.LapEventArgs e)
        {
            LapCompletedEvent?.Invoke(this, e);
        }
    }
}
