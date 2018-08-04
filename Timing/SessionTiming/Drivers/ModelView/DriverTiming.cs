namespace SecondMonitor.Timing.SessionTiming.Drivers.ModelView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using ViewModel;

    public class DriverTiming
    {

        private static readonly TimeSpan MaxPendingStateWait = TimeSpan.FromSeconds(5);

        public class LapEventArgs : EventArgs
        {
            public LapEventArgs(LapInfo lapInfo)
            {
                Lap = lapInfo;
            }

            public LapInfo Lap
            {
                get;
            }
        }

        private readonly Velocity _maximumVelocity = Velocity.FromMs(85);
        private readonly List<LapInfo> _lapsInfo;
        private readonly List<PitStopInfo> _pitStopInfo;
        private double _previousTickLapDistance;

        private SectorTiming _bestSector1;
        private SectorTiming _bestSector2;
        private SectorTiming _bestSector3;

        public DriverTiming(DriverInfo driverInfo, SessionTiming session)
        {
            _lapsInfo = new List<LapInfo>();
            _pitStopInfo = new List<PitStopInfo>();
            DriverInfo = driverInfo;
            Pace = TimeSpan.Zero;
            LapPercentage = 0;
            _previousTickLapDistance = 0;
            Session = session;
        }

        public event EventHandler<LapEventArgs> NewLapStarted;
        public event EventHandler<LapEventArgs> LapInvalidated;
        public event EventHandler<LapEventArgs> LapCompleted;

        public event EventHandler<LapInfo.SectorCompletedArgs> SectorCompletedEvent;

        public bool InvalidateFirstLap { get; set; }

        public SessionTiming Session { get; private set;}

        public DriverInfo DriverInfo { get; internal set; }

        public bool IsPlayer => DriverInfo.IsPlayer;

        public string Name => DriverInfo.DriverName;

        public int Position => DriverInfo.Position;

        public int CompletedLaps => DriverInfo.CompletedLaps;

        public bool InPits { get; private set; }

        public TimeSpan Pace { get; private set; }

        public bool IsCurrentLapValid => CurrentLap?.Valid ?? false;

        public double TotalDistanceTraveled => DriverInfo.TotalDistance;

        public bool IsLapped => (Session.SessionType == SessionType.Race || CurrentLap == null) ? DriverInfo.IsBeingLappedByPlayer : !CurrentLap.Valid;

        public bool IsLapping => DriverInfo.IsLappingPlayer;

        public string DistanceToPits => DriverInfo.DriverDebugInfo.DistanceToPits.ToString("N2");

        public LapInfo BestLap { get; private set; }

        public int PitCount => _pitStopInfo.Count;

        public PitStopInfo LastPitStopStop => _pitStopInfo.Count != 0 ? _pitStopInfo[_pitStopInfo.Count - 1] : null;

        public double LapPercentage { get; private set; }

        public double DistanceToPlayer => DriverInfo.DistanceToPlayer;

        public string CarName => DriverInfo.CarName;

        public int PaceLaps => Session.PaceLaps;

        public bool IsLastLapBestLap => BestLap != null && BestLap == LastLap;

        public bool IsActive { get; set; } = true;

        public SectorTiming BestSector1
        {
            get => _bestSector1;
            private set
            {
                PreviousBestSector1 = BestSector1;
                _bestSector1 = value;
            }

        }

        public SectorTiming BestSector2
        {
            get => _bestSector2;
            private set
            {
                PreviousBestSector2 = BestSector2;
                _bestSector2 = value;
            }

        }

        public SectorTiming BestSector3
        {
            get => _bestSector3;
            private set
            {
                PreviousBestSector3 = BestSector3;
                _bestSector3 = value;
            }

        }

        public SectorTiming PreviousBestSector1 { get; private set; }

        public SectorTiming PreviousBestSector2 { get; private set; }

        public SectorTiming PreviousBestSector3 { get; private set; }

        public IReadOnlyCollection<LapInfo> Laps => _lapsInfo.AsReadOnly();


        public bool IsLastLapBestSessionLap
        {
            get
            {
                if (LastLap == null)
                {
                    return false;
                }

                return LastLap == Session.BestSessionLap;
            }
        }

        public string Remark => DriverInfo.FinishStatus.ToString();

        public string Speed => DriverInfo.Speed.InKph.ToString("N0");

        public Velocity TopSpeed { get; private set; } = Velocity.Zero;

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.ToString("mm\\:ss\\.fff");
        }

        public static string FormatTimeSpanOnlySeconds(TimeSpan timeSpan)
        {

            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + timeSpan.ToString("ss\\.fff");
            }
            else
            {
                return "+" + timeSpan.ToString("ss\\.fff");
            }
        }

        public static DriverTiming FromModel(DriverInfo modelDriverInfo, SessionTiming session, bool invalidateFirstLap)
        {
            var driver = new DriverTiming(modelDriverInfo, session);
            driver.InvalidateFirstLap = invalidateFirstLap;
            return driver;
        }

        public bool UpdateLaps(SimulatorDataSet set)
        {
            SessionInfo sessionInfo = set.SessionInfo;
            if (!sessionInfo.IsActive)
            {
                return false;
            }

            if (sessionInfo.SessionPhase == SessionPhase.Countdown)
            {
                return false;
            }

            if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && CurrentLap != null && CurrentLap.Completed)
            {
                return false;
            }

            if (TopSpeed < DriverInfo.Speed && DriverInfo.Speed < _maximumVelocity )
            {
                TopSpeed = DriverInfo.Speed;
            }

            UpdateInPitsProperty(set);
            if (_lapsInfo.Count == 0)
            {
                LapInfo firstLap =
                    new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1, this, true, null)
                        {
                            Valid =
                                !InvalidateFirstLap
                        };
                firstLap.SectorCompletedEvent += LapSectorCompletedEvent;
                firstLap.LapInvalidatedEvent += LapInvalidatedHandler;
                firstLap.LapCompletedEvent += LapCompletedHandler;
                _lapsInfo.Add(firstLap);
                OnNewLapStarted(new LapEventArgs(firstLap));
            }

            if (LastLap != null && LastLap.IsPending && !LastLap.UpdatePendingState(set, DriverInfo))
            {
                FinishLap(LastLap, set);
            }

            LapInfo currentLap = CurrentLap;
            if (!currentLap.Completed)
            {
                UpdateCurrentLap(set);
            }

            if (ShouldFinishLap(set, currentLap))
            {
                if (!currentLap.SwitchToPendingIfNecessary(set, DriverInfo))
                {
                    FinishLap(currentLap, set);
                }
                CreateNewLap(set, currentLap);
                _previousTickLapDistance = DriverInfo.LapDistance;
                return currentLap.Valid;
            }

            _previousTickLapDistance = DriverInfo.LapDistance;
            return false;
        }

        private void LapCompletedHandler(object sender, LapEventArgs e)
        {
            OnLapCompleted(e);
        }

        private void LapInvalidatedHandler(object sender, LapEventArgs e)
        {
            OnLapInvalidated(e);
        }


        private bool ShouldFinishLap(SimulatorDataSet dataSet, LapInfo currentLap)
        {
            SessionInfo sessionInfo = dataSet.SessionInfo;

            // Use completed laps indication to end lap, when we use the sim provided lap times. This gives us the biggest assurance that lap time is already properly set
            if (dataSet.SimulatorSourceInfo.HasLapTimeInformation && currentLap.LapNumber < DriverInfo.CompletedLaps + 1)
            {
                return true;
            }

            if ((!dataSet.SimulatorSourceInfo.HasLapTimeInformation || dataSet.SimulatorSourceInfo.SimNotReportingEndOfOutLapCorrectly) && (DriverInfo.LapDistance - _previousTickLapDistance < sessionInfo.TrackInfo.LayoutLength * -0.90))
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && currentLap.PitLap && _previousTickLapDistance < DriverInfo.LapDistance && SessionType.Race != sessionInfo.SessionType && !DriverInfo.InPits)
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && SessionType.Race == sessionInfo.SessionType && !DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }

            // Driver is DNF/DQ -> finish timed lap, and set it to invalid
            if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None)
            {
                CurrentLap.Valid = false;
                return true;
            }

            return false;
        }

        private void UpdateCurrentLap(SimulatorDataSet dataSet)
        {
            CurrentLap.Tick(dataSet, DriverInfo);
            CurrentLap.InvalidBySim = !DriverInfo.CurrentLapValid;
            LapPercentage = (DriverInfo.LapDistance / dataSet.SessionInfo.TrackInfo.LayoutLength) * 100;
            if (CurrentLap.Valid && SessionType.Race != dataSet.SessionInfo.SessionType && (InPits || !DriverInfo.CurrentLapValid) && _lapsInfo.Count >= 1)
            {
                CurrentLap.Valid = false;
            }
        }


        private void FinishLap(LapInfo lapToFinish, SimulatorDataSet dataSet)
        {
            lapToFinish.FinishLap(dataSet, DriverInfo);
            lapToFinish.SectorCompletedEvent -= LapSectorCompletedEvent;
            lapToFinish.LapCompletedEvent -= LapCompletedHandler;
            lapToFinish.LapInvalidatedEvent -= LapInvalidatedHandler;

            if (lapToFinish.LapTime == TimeSpan.Zero)
            {
                lapToFinish.Valid = false;
                RevertSectorChanges(lapToFinish);
            }
            if (lapToFinish.Valid && lapToFinish.LapTime != TimeSpan.Zero && (BestLap == null || lapToFinish.LapTime < BestLap.LapTime ))
            {
                BestLap = lapToFinish;
            }

            OnLapCompleted(new LapEventArgs(lapToFinish));

            ComputePace();
        }

        private void CreateNewLap(SimulatorDataSet dataSet, LapInfo lapToCreateFrom)
        {
            if ((DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Na
                || DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.None) && dataSet.SessionInfo.SessionPhase != SessionPhase.Checkered  )
            {
                var newLap = new LapInfo(
                    dataSet.SessionInfo.SessionTime,
                    DriverInfo.CompletedLaps + 1,
                    this,
                    lapToCreateFrom);
                newLap.SectorCompletedEvent += LapSectorCompletedEvent;
                newLap.LapCompletedEvent += LapCompletedHandler;
                newLap.LapInvalidatedEvent += LapInvalidatedHandler;
                _lapsInfo.Add(newLap);
                OnNewLapStarted(new LapEventArgs(newLap));
            }
        }

        private void LapSectorCompletedEvent(object sender, LapInfo.SectorCompletedArgs e)
        {
            SectorTiming completedSector = e.SectorTiming;
            switch (completedSector.SectorNumber)
            {
                case 1:
                    if ((BestSector1 == null || BestSector1.Duration > completedSector.Duration) && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector1 = completedSector;
                    }
                    break;
                case 2:
                    if ((BestSector2 == null || BestSector2.Duration > completedSector.Duration)  && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector2 = completedSector;
                    }
                    break;
                case 3:
                    if ((BestSector3 == null || BestSector3.Duration > completedSector.Duration)  && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector3 = completedSector;
                    }
                    break;
            }
            OnSectorCompletedEvent(e);
        }

        private void UpdateInPitsProperty(SimulatorDataSet set)
        {
            if(InPits && !LastPitStopStop.Completed )
            {
                LastPitStopStop.Tick(set);
                if (CurrentLap != null)
                {
                    CurrentLap.PitLap = true;
                }
            }

            if (!InPits && DriverInfo.InPits)
            {
                InPits = true;
                if( CurrentLap != null)
                {
                    CurrentLap.PitLap = true;
                }

                _pitStopInfo.Add(new PitStopInfo(set, this, CurrentLap));
            }

            if(InPits && !DriverInfo.InPits)
            {
                InPits = false;
            }
        }


        private void ComputePace()
        {
            if(LastCompletedLap == null)
            {
                Pace = TimeSpan.Zero;
                return;
            }

            int totalPaceLaps = 0;
            TimeSpan pace = TimeSpan.Zero;
            for(int i = _lapsInfo.Count - 2; i >= 0 && totalPaceLaps < PaceLaps; i--)
            {
                LapInfo lap = _lapsInfo[i];
                if (!lap.Valid)
                {
                    continue;
                }

                pace = pace.Add(lap.LapTime);
                totalPaceLaps++;
            }

            Pace = totalPaceLaps == 0 ? TimeSpan.Zero : new TimeSpan(pace.Ticks / totalPaceLaps);
        }

        public LapInfo CurrentLap
        {
            get
            {
                if (_lapsInfo.Count == 0)
                {
                    return null;
                }

                return _lapsInfo.Last();
            }
        }

        public string LastPitInfo
        {
            get
            {
                if (DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Dnf)
                {
                    return string.Empty;
                }
                if(Session.SessionType != SessionType.Race)
                {
                    if (InPits)
                    {
                        return "In Pits";
                    }
                    else
                    {
                        return "Out";
                    }
                }

                if (LastPitStopStop == null)
                {
                    return "0";
                }
                else
                {
                    return PitCount + ":(" + LastPitStopStop.PitInfoFormatted + ")";
                }
            }
        }


        public LapInfo LastCompletedLap
        {
            get
            {
                if (_lapsInfo.Count < 2)
                {
                    return null;
                }

                for (int i = _lapsInfo.Count - 2; i >= 0; i--)
                {
                    if (_lapsInfo[i].Valid)
                    {
                        return _lapsInfo[i];
                    }
                }

                return null;
            }
        }

        public LapInfo LastLap
        {
            get
            {
                if (_lapsInfo.Count < 2)
                {
                    return null;
                }

                return _lapsInfo[_lapsInfo.Count - 2];
            }
        }

        protected virtual void OnSectorCompletedEvent(LapInfo.SectorCompletedArgs e)
        {
            SectorCompletedEvent?.Invoke(this, e);
        }

        protected virtual void OnNewLapStarted(LapEventArgs e)
        {
            NewLapStarted?.Invoke(this, e);
        }

        protected virtual void OnLapInvalidated(LapEventArgs e)
        {
            RevertSectorChanges(e.Lap);
            LapInvalidated?.Invoke(this, e);
        }

        protected virtual void OnLapCompleted(LapEventArgs e)
        {
            LapCompleted?.Invoke(this, e);
        }

        private void RevertSectorChanges(LapInfo lap)
        {
            if (BestSector1 != null && BestSector1 == lap.Sector1)
            {
                _bestSector1 = PreviousBestSector1;
            }

            if (BestSector2 != null && BestSector2 == lap.Sector2)
            {
                _bestSector2 = PreviousBestSector2;
            }

            if (BestSector3 != null && BestSector3 == lap.Sector3)
            {
                _bestSector3 = PreviousBestSector3;
            }
        }
    }
}
