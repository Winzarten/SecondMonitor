using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class DriverTiming
    {
        private List<LapInfo> _lapsInfo;
        private List<PitStopInfo> _pitStopInfo;
        private Single _previousTickLapDistance;
        private readonly Velocity _maximumVelocity = Velocity.FromMs(85);


        public DriverTiming(DriverInfo driverInfo, SessionTiming session)
        {
            _lapsInfo = new List<LapInfo>();
            _pitStopInfo = new List<PitStopInfo>();
            DriverInfo = driverInfo;            
            Pace = new TimeSpan(0);
            LapPercentage = 0;
            _previousTickLapDistance = 0;
            Session = session;
        }
        
        public bool InvalidateFirstLap { get; set; }
        private SessionTiming Session { get; set;}
        public DriverInfo DriverInfo { get; internal set; }
        public bool IsPlayer { get => DriverInfo.IsPlayer; }
        public string Name { get => DriverInfo.DriverName; }
        public int Position { get => DriverInfo.Position; }
        public int CompletedLaps { get => DriverInfo.CompletedLaps; }
        public bool InPits { get; private set; }
        public TimeSpan Pace { get; private set; }
        public string PaceAsString
        {
            get
            {
                if (DriverInfo.IsPlayer || !Session.DisplayBindTimeRelative || Session.Player.Pace == TimeSpan.Zero)
                {
                    return FormatTimeSpan(Pace);
                }
                else
                {
                    return FormatTimeSpanOnlySeconds(Pace.Subtract(Session.Player.Pace));
                }
            }
        }
        public bool IsCurrentLapValid { get => CurrentLap != null ? CurrentLap.Valid : false; }
        public double TotalDistanceTraveled { get => DriverInfo.TotalDistance; }
        public bool IsLapped { get => DriverInfo.IsBeingLappedByPlayer; }
        public bool IsLapping { get => DriverInfo.IsLapingPlayer; }
        public string DistanceToPits
        {
            get => DriverInfo.DriverDebugInfo.DistanceToPits.ToString("N2");
        }
        public LapInfo BestLap { get; private set; }
        public string BestLapString
        {
            get
            {
                if (BestLap == null)
                {
                    return "N/A";
                }

                if (DriverInfo.IsPlayer || !Session.DisplayBindTimeRelative || Session.Player.BestLap == null)
                {
                    return "L" + BestLap.LapNumber + "/" + FormatTimeSpan(BestLap.LapTime);
                }
                else
                {
                    return "L" + BestLap.LapNumber + "/" + FormatTimeSpanOnlySeconds(BestLap.LapTime.Subtract(Session.Player.BestLap.LapTime));
                }
            }
        }
        public int PitCount { get => _pitStopInfo.Count; }
        public PitStopInfo LastPitStopStop { get => _pitStopInfo.Count != 0 ? _pitStopInfo[_pitStopInfo.Count - 1] : null; }
        public Single LapPercentage { get; private set; }
        public Single DistanceToPlayer { get => DriverInfo.DistanceToPlayer; }
        public string CarName { get => DriverInfo.CarName; }
        public int PaceLaps { get => Session.PaceLaps; }
            

        public bool IsLastLapBestLap { get
            {
                if (BestLap == null)
                {
                    return false;
                }

                return BestLap == LastLap;
            } }

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

        public string Remark
        {
            get => DriverInfo.FinishStatus.ToString();
        }

        public string Speed { get => DriverInfo.Speed.InKph.ToString("N0"); }
        public  Velocity TopSpeed { get; private set; } = Velocity.Zero;
        public string TopSpeedString { get
            {
                //if (!Session.DisplayBindTimeRelative || Session.Player == null || DriverInfo.IsPlayer)
                    return TopSpeed.InKph.ToString("N0");
                //return ((TopSpeed - Session.Player.TopSpeed).InKPH.ToString("N0");
            }
        }


        public string TimeToPlayerFormatted
        {
            get
            {
                if (Session.Player == null)
                {
                    return "";
                }

                if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na)
                {
                    return DriverInfo.FinishStatus.ToString();
                }

                if (DriverInfo.IsPlayer)
                {
                    return "";
                }

                if (Session.LastSet.SessionInfo.SessionType != SessionInfo.SessionTypeEnum.Race)
                {
                    return "";
                }

                double distanceToUse;
                if (Session.DisplayGapToPlayerRelative)
                {
                    distanceToUse = DriverInfo.DistanceToPlayer;
                }
                else
                {
                    distanceToUse = Session.Player.TotalDistanceTraveled - TotalDistanceTraveled;
                }

                if (Math.Abs(distanceToUse)> Session.LastSet.SessionInfo.LayoutLength)
                {
                    return ((int)(distanceToUse) / (int)Session.LastSet.SessionInfo.LayoutLength) +"LAP";
                }

                if (distanceToUse > 0)
                {
                    double requiredTime = distanceToUse / (DriverInfo.Speed.InMs);
                    if (requiredTime < 30)
                    {
                        return FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime));
                    }
                    else
                    {
                        return "+30.000+";
                    }
                }
                else
                {
                    double requiredTime = distanceToUse / (Session.Player.DriverInfo.Speed.InMs);
                    if (requiredTime > -30)
                    {
                        return FormatTimeSpanOnlySeconds(TimeSpan.FromSeconds(requiredTime));
                    }
                    else
                    {
                        return "-30.000+";
                    }
                }
            }
        }


        public bool UpdateLaps(SimulatorDataSet set)
        {
            SessionInfo sessionInfo = set.SessionInfo;
            if (!sessionInfo.IsActive)
            {
                return false;
            }

            if (sessionInfo.SessionPhase == SessionInfo.SessionPhaseEnum.Countdown)
            {
                return false;
            }

            if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.Na && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && LastLap != null && LastLap.LapEnd != TimeSpan.Zero)
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
                LapInfo firstLap = new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1, this, true);
                firstLap.Valid = !InvalidateFirstLap;
                _lapsInfo.Add(firstLap);
            }
            LapInfo currentLap = CurrentLap;
            if (!currentLap.Completed)
            {
                UpdateCurrentLap(set);
            }
            if (ShouldFinishLap(set, currentLap))
            {
                FinishCurrentLap(set);
                _previousTickLapDistance = DriverInfo.LapDistance;
                return currentLap.Valid;
            }
            _previousTickLapDistance = DriverInfo.LapDistance;
            return false;
        }

        private bool ShouldFinishLap(SimulatorDataSet dataSet, LapInfo currentLap)
        {
            SessionInfo sessionInfo = dataSet.SessionInfo;
            //Use completed laps indication to end lap, when we use the sim provided lap times. This gives us the biggest assurance that lap time is already properly set
            if (dataSet.SimulatorSourceInfo.HasLapTimeInformation && currentLap.LapNumber < DriverInfo.CompletedLaps + 1)
            {
                return true;
            }
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation &&  (DriverInfo.LapDistance - _previousTickLapDistance < sessionInfo.LayoutLength * -0.90 ))
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && currentLap.PitLap && _previousTickLapDistance < DriverInfo.LapDistance && SessionInfo.SessionTypeEnum.Race != sessionInfo.SessionType)
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && SessionInfo.SessionTypeEnum.Race == sessionInfo.SessionType && !DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }
            //Driver is DNF/DQ -> finish timed lap, and set it to invalid
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
            LapPercentage = (DriverInfo.LapDistance / dataSet.SessionInfo.LayoutLength)*100;
            if (SessionInfo.SessionTypeEnum.Race != dataSet.SessionInfo.SessionType && ((!IsPlayer && InPits) || !DriverInfo.CurrentLapValid) && _lapsInfo.Count > 1)
            {
                CurrentLap.Valid = false;
            }
        }
        

        private void FinishCurrentLap(SimulatorDataSet dataSet)
        {
            CurrentLap.FinishLap(dataSet, DriverInfo);
            if (CurrentLap.Valid && (BestLap == null || CurrentLap.LapTime < BestLap.LapTime ))
            {
                BestLap = CurrentLap;
            }

            if (DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.Na || DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.None)
            {
                _lapsInfo.Add(new LapInfo(dataSet.SessionInfo.SessionTime, DriverInfo.CompletedLaps + 1,this));
            }

            ComputePace();
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
                if(CurrentLap!=null)
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
            if(LastCompletedLap== null)
            {
                Pace = new TimeSpan(0);
                return;
            }
            int totalPaceLaps = 0;
            TimeSpan pace = new TimeSpan(0);
            for(int i = _lapsInfo.Count -2; i>=0 && totalPaceLaps < PaceLaps; i--)
            {
                LapInfo lap = _lapsInfo[i];
                if (!lap.Valid)
                {
                    continue;
                }

                pace = pace.Add(lap.LapTime);
                totalPaceLaps++;
            }
            if (totalPaceLaps == 0)
            {
                Pace = new TimeSpan(0);
            }
            else
            {
                Pace = new TimeSpan(pace.Ticks / totalPaceLaps);
            }
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
                if(Session.SessionType != SessionInfo.SessionTypeEnum.Race)
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
        public string CurrentLapProgressTime
        {
            get
            {
                if (CurrentLap == null)
                {
                    return "";
                }

                if (!CurrentLap.Valid)
                {
                    return "Lap Invalid";
                }

                TimeSpan progress = CurrentLap.LapProgressTime;
                return FormatTimeSpan(progress);
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

        public string LastLapTime
        {
            get
            {
                LapInfo lastCompletedLap = LastCompletedLap;
                if (lastCompletedLap != null)
                {
                    if(DriverInfo.IsPlayer || !Session.DisplayBindTimeRelative || Session.Player.LastCompletedLap == null)
                    {
                        return FormatTimeSpan(lastCompletedLap.LapTime);
                    }
                    else
                    {
                        return FormatTimeSpanOnlySeconds(lastCompletedLap.LapTime.Subtract(Session.Player.LastCompletedLap.LapTime));
                    }
                }
                else
                {
                    return "N/A";
                }
            }
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            //String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }
        public static string FormatTimeSpanOnlySeconds(TimeSpan timeSpan)
        {
            //return "FOO";
            //String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
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
    }
}
