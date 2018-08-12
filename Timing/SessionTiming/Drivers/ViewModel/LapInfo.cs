namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.DataModel.Telemetry;

    public class LapInfo
    {
        internal enum CompletionMethod
        {
            None,
            ByLapNumber,
            ByCrossingTheLine,
            ByChangingValidity,
            ByChangingValidity2,
            ByChangingValidity3,

        }

        private static readonly Distance MaxDistancePerTick = Distance.FromMeters(300);
        private static readonly TimeSpan MaxPendingTime = TimeSpan.FromSeconds(3);

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
        private bool _isPending;
        private TimeSpan _isPendingStart;
        private DriverInfo _previousDriverInfo;
        internal CompletionMethod LapCompletionMethod { get; set; } = CompletionMethod.None;

        public LapInfo(SimulatorDataSet dataSet, int lapNumber, DriverTiming driver, LapInfo previousLapInfo) :
            this(dataSet, lapNumber, driver, false, previousLapInfo)
        {
        }

        public LapInfo(SimulatorDataSet dataSet, int lapNumber, DriverTiming driver, bool firstLap, LapInfo previousLapInfo)
        {
            Driver = driver;
            LapStart = dataSet.SessionInfo.SessionTime;
            LapProgressTimeBySim = TimeSpan.Zero;
            LapProgressTimeByTiming = TimeSpan.Zero;
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = firstLap;
            PitLap = false;
            PreviousLap = previousLapInfo;
            CompletedDistance = double.NaN;
            PortionTimes = new LapPortionTimes(5, dataSet.SessionInfo.TrackInfo.LayoutLength, this);
        }

        public event EventHandler<SectorCompletedArgs> SectorCompletedEvent;

        public event EventHandler<DriverTiming.LapEventArgs> LapInvalidatedEvent;
        public event EventHandler<DriverTiming.LapEventArgs> LapCompletedEvent;

        public TimeSpan LapStart { get; }

        public int LapNumber { get; private set; }

        public TelemetrySnapshot LapEndSnapshot { get; private set; }

        public double CompletedDistance { get; private set; }

        public bool Valid
        {
            get => _valid;
            set
            {
                if (Valid && !value)
                {
                    _valid = false;
                    OnLapInvalidatedEvent(new DriverTiming.LapEventArgs(this));
                }
                else
                {
                    _valid = value;
                }

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

        public LapPortionTimes PortionTimes { get; }

        public LapInfo PreviousLap { get; }

        public SectorTiming Sector1 { get; private set; }

        public SectorTiming Sector2 { get; private set; }

        public SectorTiming Sector3 { get; private set; }

        public TimeSpan LapEnd { get; private set; }

        public TimeSpan LapTime { get; private set; } = TimeSpan.Zero;

        public TimeSpan LapProgressTimeByTiming { get; private set; }

        private bool LapProgressTimeBySimInitialized { get; set; }

        public TimeSpan LapProgressTimeBySim { get; private set; }

        public TimeSpan CurrentlyValidProgressTime =>
            LapProgressTimeBySimInitialized ? LapProgressTimeBySim : LapProgressTimeByTiming;

        public SectorTiming CurrentSector { get; private set; }

        private PendingSector PendingSector { get; set; }

        public bool IsPending => _isPending || PendingSector != null;

        public static Func<LapInfo, SectorTiming> Sector1SelFunc => x => x.Sector1;
        public static Func<LapInfo, SectorTiming> Sector2SelFunc => x => x.Sector2;
        public static Func<LapInfo, SectorTiming> Sector3SelFunc => x => x.Sector3;

        public void FinishLap(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            TimeSpan lapDurationByTiming = dataSet.SessionInfo.SessionTime.Subtract(LapStart);

            // Perform a sanity check on the sim reported lap time. The time difference between what the application counted and the sim counted cannot be more than 5 seconds.
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation || Math.Abs(lapDurationByTiming.TotalSeconds - driverInfo.Timing.LastLapTime.TotalSeconds) > 5)
            {
                LapEnd = _isPendingStart != TimeSpan.Zero ? _isPendingStart : dataSet.SessionInfo.SessionTime;
            }
            else
            {
                LapEnd = LapStart.Add(driverInfo.Timing.LastLapTime);
            }

            LapTime = LapEnd.Subtract(LapStart);
            if (LapTime == TimeSpan.Zero || CompletedDistance < dataSet.SessionInfo.TrackInfo.LayoutLength * 0.8)
            {
                Valid = false;
            }
            LapEndSnapshot = new TelemetrySnapshot(driverInfo, dataSet.SessionInfo.WeatherInfo);
            Completed = true;
        }

        public void Tick(SimulatorDataSet dataSet, DriverInfo driverInfo)

        {
            UpdateLapProgressTimeBySim(dataSet, driverInfo);
            LapProgressTimeByTiming = dataSet.SessionInfo.SessionTime.Subtract(LapStart);


            // Let 5 seconds for the source data noise, when lap count might not be properly updated at instance creation
            if (LapProgressTimeByTiming.TotalSeconds < 5 && LapNumber != driverInfo.CompletedLaps + 1)
            {
                LapNumber = driverInfo.CompletedLaps + 1;
            }

            if (dataSet.SimulatorSourceInfo.SectorTimingSupport != DataInputSupport.NONE)
            {
                TickSectors(dataSet, driverInfo);
            }

            if (IsMaxSpeedViolated(driverInfo))
            {
                Valid = false;
            }

            // driverInfo.LapDistance might still hold value from previous lap at this point, so wait until it is reasonably small before starting to compute complete distance
            if (double.IsNaN(CompletedDistance) && driverInfo.LapDistance < 500)
            {
                CompletedDistance = driverInfo.LapDistance;
            }

            if (CompletedDistance != double.NaN && CompletedDistance < driverInfo.LapDistance)
            {
                CompletedDistance = driverInfo.LapDistance;
                if (Driver.IsPlayer)
                {
                    PortionTimes.UpdateLapPortions();
                }
            }

            _previousDriverInfo = driverInfo;
        }

        private void UpdateLapProgressTimeBySim(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            TimeSpan newLapProgressTimeBySim = driverInfo.Timing.CurrentLapTime;

            // sim is reporting nonsense as our current lap time
            if (newLapProgressTimeBySim <= TimeSpan.Zero)
            {
                LapProgressTimeBySimInitialized = false;
                return;
            }

            // We were unable to use the sim provided lap time for 75% for the lap. I thing it is safe to say that this boat has passed
            if (!LapProgressTimeBySimInitialized && CompletedDistance > dataSet.SessionInfo.TrackInfo.LayoutLength * 0.75)
            {
                return;
            }

            // It is not possible to use the lap time provided by the sim from the beginning, because it might still contain value from the previous lap. The sim has 5 second window to update the value to correct time
            // Only after that is done we can use the lap provided lap time
            if (!LapProgressTimeBySimInitialized && newLapProgressTimeBySim < TimeSpan.FromSeconds(5))
            {
                LapProgressTimeBySim = newLapProgressTimeBySim;
                LapProgressTimeBySimInitialized = true;
                return;
            }

            // Sim reported lap time is smaller than the previously reported lap time, that doesn't sound good - we're unable to use the lap time
            if (LapProgressTimeBySimInitialized && newLapProgressTimeBySim < LapProgressTimeByTiming)
            {
                LapProgressTimeBySimInitialized = false;
                return;
            }

            // Huuray, sanity checks met, we're able to use the lap time provided by sim
            if (LapProgressTimeBySimInitialized)
            {
                LapProgressTimeBySim = newLapProgressTimeBySim;
            }
        }

        private bool IsMaxSpeedViolated(DriverInfo currentDriverInfo)
        {
            if (_previousDriverInfo == null)
            {
                return false;
            }

            Distance distance = Point3D.GetDistance(currentDriverInfo.WorldPosition, _previousDriverInfo.WorldPosition);
            return distance.DistanceInM > MaxDistancePerTick.DistanceInM;
        }

        private void TickSectors(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (dataSet.SimulatorSourceInfo.SectorTimingSupport == DataInputSupport.PLAYER_ONLY && !driverInfo.IsPlayer)
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

            UpdatePendingState(dataSet, driverInfo);

            SectorTiming shouldBeActive = PickSector(driverInfo.Timing.CurrentSector);
            if (shouldBeActive == CurrentSector)
            {
                CurrentSector.Tick(dataSet, driverInfo);
                return;
            }

            FinishSectorAndCheckIfPending(CurrentSector, driverInfo, dataSet);
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

        private void FinishSectorAndCheckIfPending(
            SectorTiming sectorTiming,
            DriverInfo driverInfo,
            SimulatorDataSet dataSet)
        {
            if (SectorTiming.PickTimingFormDriverInfo(driverInfo, sectorTiming.SectorNumber) <= TimeSpan.Zero)
            {
                InitPendingSector(CurrentSector, driverInfo, dataSet);
            }
            else
            {
                FinishSector(sectorTiming, driverInfo, dataSet);
            }
        }

        private void FinishSector(SectorTiming sectorTiming, DriverInfo driverInfo, SimulatorDataSet dataSet)
        {
            sectorTiming.Finish(driverInfo);
            if (sectorTiming.Duration == TimeSpan.Zero || (Driver.InPits && dataSet.SessionInfo.SessionType != SessionType.Race))
            {
                Valid = false;
            }
            OnSectorCompleted(new SectorCompletedArgs(sectorTiming));
        }

        private void InitPendingSector(SectorTiming sectorTiming, DriverInfo driverInfo, SimulatorDataSet dataSet)
        {
            if (PendingSector?.Sector == sectorTiming)
            {
                return;
            }

            if (PendingSector != null)
            {
                FinishSector(PendingSector.Sector, driverInfo, dataSet);
            }
            PendingSector = new PendingSector(sectorTiming, dataSet.SessionInfo.SessionTime);
        }


        // R3E has some weird behavior that sometimes it doesn't report last lap time correctly (reports zero), do not end lap in such weird cases
        public bool SwitchToPendingIfNecessary(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (dataSet.SimulatorSourceInfo.HasLapTimeInformation && driverInfo.Timing.LastLapTime == TimeSpan.Zero)
            {
                _isPending = true;
                _isPendingStart = dataSet.SessionInfo.SessionTime;
            }
            return _isPending;
        }

        public bool UpdatePendingState(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (!IsPending)
            {
                return false;
            }

            if (PendingSector != null && (PendingSector.HasValidTimingInformation(driverInfo) || PendingSector.HasTimedOut(dataSet.SessionInfo.SessionTime)))
            {
                FinishSector(PendingSector.Sector, driverInfo, dataSet);
                PendingSector = null;
            }

            if (_isPending && (driverInfo.Timing.LastLapTime != TimeSpan.Zero || dataSet.SessionInfo.SessionTime - _isPendingStart > MaxPendingTime))
            {
                _isPending = false;
            }
            return IsPending;
        }

        /*
         * Performs a sanity check on the laps data to evaluate if the lap really make sense (i.e. if it wasn't terminated prematurely by the sim
         */
        public bool IsLapDataSane(SimulatorDataSet dataSet)
        {
            // Not completed laps are always sane
            if (!Completed)
            {
                return true;
            }

            // Lap data is sane if we completed at least 50% of the track and the lap hes run for more than 10 seconds
            return CompletedDistance > dataSet.SessionInfo.TrackInfo.LayoutLength * 0.5 && LapProgressTimeByTiming > TimeSpan.FromSeconds(10);
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
