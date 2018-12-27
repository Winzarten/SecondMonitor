using System.Linq;

namespace SecondMonitor.Timing.SessionTiming.Drivers.ViewModel
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public class LapInfo
    {
       private static readonly Distance MaxDistancePerTick = Distance.FromMeters(300);
        private static readonly TimeSpan MaxPendingTime = TimeSpan.FromSeconds(2);

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
        internal LapCompletionMethod LapCompletionMethod { get; set; } = LapCompletionMethod.None;

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
            LapTelemetryInfo = new LapTelemetryInfo(driver.DriverInfo, dataSet, this, Driver.Session.TimingDataViewModel.DisplaySettingsViewModel.TelemetrySettingsViewModel.IsTelemetryLoggingEnabled, TimeSpan.FromMilliseconds(Driver.Session.TimingDataViewModel.DisplaySettingsViewModel.TelemetrySettingsViewModel.LoggingInterval));
        }

        public event EventHandler<SectorCompletedArgs> SectorCompletedEvent;

        public event EventHandler<LapEventArgs> LapInvalidatedEvent;
        public event EventHandler<LapEventArgs> LapCompletedEvent;

        public TimeSpan LapStart { get; }

        public int LapNumber { get; private set; }

        public double CompletedDistance { get; private set; }

        public bool Valid
        {
            get => _valid;
            set
            {
                if (Valid && !value)
                {
                    _valid = false;
                    OnLapInvalidatedEvent(new LapEventArgs(this));
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
                    OnLapCompletedEvent(new LapEventArgs(this));
                }
            }

        }

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

        public LapTelemetryInfo LapTelemetryInfo { get; }

        public static Func<LapInfo, SectorTiming> Sector1SelFunc => x => x.Sector1;
        public static Func<LapInfo, SectorTiming> Sector2SelFunc => x => x.Sector2;
        public static Func<LapInfo, SectorTiming> Sector3SelFunc => x => x.Sector3;

        public void FinishLap(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            TimeSpan lapDurationByTiming = dataSet.SessionInfo.SessionTime.Subtract(LapStart);

            // Perform a sanity check on the sim reported lap time. The time difference between what the application counted and the sim counted cannot be more than 15 seconds.
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation || Math.Abs(lapDurationByTiming.TotalSeconds - driverInfo.Timing.LastLapTime.TotalSeconds) > 15)
            {
                LapEnd = _isPendingStart != TimeSpan.Zero ? _isPendingStart : dataSet.SessionInfo.SessionTime;
            }
            else
            {
                LapEnd = LapStart.Add(driverInfo.Timing.LastLapTime);
            }

            LapTime = LapEnd.Subtract(LapStart);
            SectorTiming[] sectors = {Sector1, Sector2, Sector3};
            if (LapTime == TimeSpan.Zero || CompletedDistance < dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters * 0.8 || (sectors.Any(x => x?.Duration != TimeSpan.Zero) && sectors.Any(x=> x == null || x.Duration == TimeSpan.Zero)))
            {
                Valid = false;
            }

            LapTelemetryInfo.CreateLapEndSnapshot(driverInfo, dataSet.SessionInfo.WeatherInfo, dataSet.InputInfo);

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

            if (dataSet.SimulatorSourceInfo.SectorTimingSupport != DataInputSupport.None)
            {
                TickSectors(dataSet, driverInfo);
            }

            if (IsMaxSpeedViolated(driverInfo))
            {
                Valid = false;
            }

            // driverInfo.TraveledDistance might still hold value from previous lap at this point, so wait until it is reasonably small before starting to compute complete distance.
            // Allow 90% of layout length, as some AC tracks have pit exit before the lap end.
            if (double.IsNaN(CompletedDistance) && driverInfo.LapDistance < dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters * 0.9)
            {
                CompletedDistance = driverInfo.LapDistance;
            }

            if (!double.IsNaN(CompletedDistance))
            {
                if (CompletedDistance <= driverInfo.LapDistance)
                {
                    CompletedDistance = driverInfo.LapDistance;
                }
                if (Driver.IsPlayer)
                {
                    LapTelemetryInfo.UpdateTelemetry(dataSet);
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
            if (!LapProgressTimeBySimInitialized && CompletedDistance > dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters * 0.75)
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

            double lapDistance = Math.Abs(_previousDriverInfo.LapDistance - currentDriverInfo.LapDistance);
            Distance distance = Point3D.GetDistance(currentDriverInfo.WorldPosition, _previousDriverInfo.WorldPosition);
            return distance.InMeters > MaxDistancePerTick.InMeters || (lapDistance > 500 && lapDistance < Driver.Session.LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters * 0.8);
        }

        private void TickSectors(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (dataSet.SimulatorSourceInfo.SectorTimingSupport == DataInputSupport.PlayerOnly && !driverInfo.IsPlayer)
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
            sectorTiming.Finish(driverInfo, dataSet);
            if ((sectorTiming.Duration == TimeSpan.Zero || (Driver.InPits && dataSet.SessionInfo.SessionType != SessionType.Race)) && dataSet.SimulatorSourceInfo.InvalidateLapBySector)
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
            if (dataSet.SimulatorSourceInfo.ForceLapOverTime)
            {
                _isPending = true;
                _isPendingStart = dataSet.SessionInfo.SessionTime;
            }

            if (dataSet.SimulatorSourceInfo.HasLapTimeInformation && driverInfo.Timing.LastLapTime == TimeSpan.Zero)
            {
                _isPending = true;
                _isPendingStart = dataSet.SessionInfo.SessionTime;
            }

            if (PreviousLap != null && dataSet.SimulatorSourceInfo.HasLapTimeInformation && (driverInfo.Timing.LastLapTime == PreviousLap.LapTime || PreviousLap.LapTime == TimeSpan.Zero))
            {
                _isPending = true;
                _isPendingStart = dataSet.SessionInfo.SessionTime;
            }

            return IsPending;
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


            if (dataSet.SessionInfo.SessionTime - _isPendingStart > MaxPendingTime)
            {
                _isPending = false;
                return IsPending;
            }


            if (dataSet.SimulatorSourceInfo.ForceLapOverTime)
            {
                return IsPending;
            }

            if (_isPending && (driverInfo.Timing.LastLapTime == TimeSpan.Zero))
            {
                return IsPending;
            }

            if (PreviousLap != null && _isPending && (driverInfo.Timing.LastLapTime == PreviousLap.LapTime && PreviousLap.LapTime == TimeSpan.Zero))
            {
                return IsPending;
            }

            _isPending = false;
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

            // Lap data is sane if we completed at least 90% of the track and the lap hes run for more than 10 seconds
            return CompletedDistance > dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters * 0.9 && LapProgressTimeByTiming > TimeSpan.FromSeconds(10);
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

        protected virtual void OnLapInvalidatedEvent(LapEventArgs e)
        {
            LapInvalidatedEvent?.Invoke(this, e);
        }

        protected virtual void OnLapCompletedEvent(LapEventArgs e)
        {
            LapCompletedEvent?.Invoke(this, e);
        }
    }
}
