using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class DriverTiming
    {
        private List<LapInfo> lapsInfo;
        private List<PitInfo> pitStopInfo;
        private Single previousTickLapDistance;
        private int paceLaps;

        public DriverTiming(DriverInfo driverInfo, SessionTiming session)
        {
            lapsInfo = new List<LapInfo>();
            pitStopInfo = new List<PitInfo>();
            DriverInfo = driverInfo;
            paceLaps = 4;
            Pace = new TimeSpan(0);
            LapPercentage = 0;
            previousTickLapDistance = 0;
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
            get => DriverInfo.DistanceToPits.ToString("N2");
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
        public int PitCount { get => pitStopInfo.Count; }
        public PitInfo LastPitStop { get => pitStopInfo.Count != 0 ? pitStopInfo[pitStopInfo.Count - 1] : null; }
        public Single LapPercentage { get; private set; }
        public Single DistanceToPlayer { get => DriverInfo.DistanceToPlayer; }
        public string CarName { get => DriverInfo.CarName; }
        public int PaceLaps { get => paceLaps; set
            {
                paceLaps = value;
                ComputePace();
            }

            }

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

        public string Speed { get => DriverInfo.Speed.InKPH.ToString("N0"); }
        public  Velocity TopSpeed { get; private set; } = Velocity.Zero;
        public string TopSpeedString { get
            {
                //if (!Session.DisplayBindTimeRelative || Session.Player == null || DriverInfo.IsPlayer)
                    return TopSpeed.InKPH.ToString("N0");
                //return (TopSpeed - Session.Player.TopSpeed).InKPH.ToString("N0");
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

                if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.NA)
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

            if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.NA && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None && LastLap != null && LastLap.LapEnd != TimeSpan.Zero)
            {
                return false;
            }

            if (TopSpeed < DriverInfo.Speed)
            {
                TopSpeed = DriverInfo.Speed;
            }

            UpdateInPitsProperty(set);
            if (lapsInfo.Count == 0)
            {
                LapInfo firstLap = new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1, this, true);
                firstLap.Valid = !InvalidateFirstLap;
                lapsInfo.Add(firstLap);
            }
            LapInfo currentLap = CurrentLap;
            if (currentLap.LapNumber == DriverInfo.CompletedLaps + 1)
            {
                UpdateCurrentLap(sessionInfo);
            }
            if (ShouldFinishLap(sessionInfo, currentLap))
            {
                FinishCurrentLap(sessionInfo);
                previousTickLapDistance = DriverInfo.LapDistance;
                return currentLap.Valid;
            }
            previousTickLapDistance = DriverInfo.LapDistance;
            return false;
        }

        private bool ShouldFinishLap(SessionInfo sessionInfo, LapInfo currentLap)
        {            
            if (currentLap.LapNumber < DriverInfo.CompletedLaps + 1)
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer && currentLap.PitLap && previousTickLapDistance < DriverInfo.LapDistance && SessionInfo.SessionTypeEnum.Race != sessionInfo.SessionType)
            {
                return true;
            }

            if (!currentLap.Valid && DriverInfo.CurrentLapValid && SessionInfo.SessionTypeEnum.Race == sessionInfo.SessionType && !DriverInfo.IsPlayer && (currentLap.FirstLap && !InvalidateFirstLap))
            {
                return true;
            }

            if (DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.NA && DriverInfo.FinishStatus != DriverInfo.DriverFinishStatus.None)
            {
                return true;
            }

            return false;
        }

        private void UpdateCurrentLap(SessionInfo sessionInfo)
        {
            CurrentLap.Tick(sessionInfo.SessionTime);
            CurrentLap.InvalidBySim = !DriverInfo.CurrentLapValid;
            LapPercentage = (DriverInfo.LapDistance / sessionInfo.LayoutLength)*100;
            if (SessionInfo.SessionTypeEnum.Race != sessionInfo.SessionType && ((!IsPlayer && InPits) || !DriverInfo.CurrentLapValid) && lapsInfo.Count > 1)
            {
                CurrentLap.Valid = false;
            }
        }
        

        private void FinishCurrentLap(SessionInfo sessionInfo)
        {
            CurrentLap.FinishLap(sessionInfo.SessionTime, DriverInfo.Timing.LastLapTime);
            if (CurrentLap.Valid && (BestLap == null || CurrentLap.LapTime < BestLap.LapTime ))
            {
                BestLap = CurrentLap;
            }

            if (DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.NA || DriverInfo.FinishStatus == DriverInfo.DriverFinishStatus.None)
            {
                lapsInfo.Add(new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1,this));
            }

            ComputePace();
        }

        private void UpdateInPitsProperty(SimulatorDataSet set)
        {
            if(InPits && !LastPitStop.Completed )
            {
                LastPitStop.Tick(set);
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

                pitStopInfo.Add(new PitInfo(set, this, CurrentLap));
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
            for(int i = lapsInfo.Count -2; i>=0 && totalPaceLaps < PaceLaps; i--)
            {
                LapInfo lap = lapsInfo[i];
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
                if (lapsInfo.Count == 0)
                {
                    return null;
                }

                return lapsInfo.Last();
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
                if (LastPitStop == null)
                {
                    return "0";
                }
                else
                {
                    return PitCount + ":(" + LastPitStop.PitInfoFormatted + ")";
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
                if (lapsInfo.Count < 2)
                {
                    return null;
                }

                for (int i = lapsInfo.Count - 2; i >= 0; i--)
                {
                    if (lapsInfo[i].Valid)
                    {
                        return lapsInfo[i];
                    }
                }
                return null;
            }
        }

        public LapInfo LastLap
        {
            get
            {
                if (lapsInfo.Count < 2)
                {
                    return null;
                }

                return lapsInfo[lapsInfo.Count - 2];                
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
