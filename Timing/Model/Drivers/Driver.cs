using SecondMonitor.DataModel;
using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class Driver
    {
        private List<LapInfo> lapsInfo;        

        public Driver(DriverInfo driverInfo)
        {
            lapsInfo = new List<LapInfo>();
            DriverInfo = driverInfo;
            Pace = new TimeSpan(0);
            PitCount = 0;
        }
        public DriverInfo DriverInfo { get; internal set; }
        public bool IsPlayer { get => DriverInfo.IsPlayer; }
        public string Name { get => DriverInfo.DriverName; }
        public int Position { get => DriverInfo.Position; }
        public int CompletedLaps { get => DriverInfo.CompletedLaps; }
        public bool InPits { get; private set; }
        public TimeSpan Pace { get; private set; }
        public string PaceAsString { get => FormatTimeSpan(Pace);}
        public bool IsCurrentLapValid { get => CurrentLap != null ? CurrentLap.Valid : false; }
        public LapInfo BestLap { get; set; }
        public string BestLapString { get => BestLap != null ? FormatTimeSpan(BestLap.LapTime) : "N/A"; }
        public int PitCount { get; private set; }

        public bool IsLastLapBestLap { get
            {
                if (BestLap == null)
                    return false;
                return BestLap == LastCompletedLap;
            } }

        public bool UpdateLaps(SessionInfo sessionInfo)
        {
            if (!sessionInfo.IsActive)
                return false;
            UpdateInPitsProperty();            
            if (lapsInfo.Count == 0)
            {
                lapsInfo.Add(new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1));
            }
            LapInfo currentLap = CurrentLap;
            if (currentLap.LapNumber == DriverInfo.CompletedLaps + 1)
            {
                UpdateCurrentLap(sessionInfo);
            }
            if (currentLap.LapNumber < DriverInfo.CompletedLaps + 1 ||
                (!currentLap.Valid && DriverInfo.CurrentLapValid && SessionInfo.SessionTypeEnum.Race == sessionInfo.SessionType && !DriverInfo.IsPlayer)
                || (!currentLap.Valid && DriverInfo.CurrentLapValid && DriverInfo.IsPlayer))
            {
                FinishCurrentLap(sessionInfo);
                return currentLap.Valid;
            }
            return false;
        }

        private void UpdateCurrentLap(SessionInfo sessionInfo)
        {
            CurrentLap.Tick(sessionInfo.SessionTime);
            if (SessionInfo.SessionTypeEnum.Race != sessionInfo.SessionType && ((!IsPlayer && InPits) || !DriverInfo.CurrentLapValid))
                CurrentLap.Valid = false;
        }

        private void FinishCurrentLap(SessionInfo sessionInfo)
        {
            CurrentLap.FinishLap(sessionInfo.SessionTime);
            if (BestLap == null || CurrentLap.LapTime < BestLap.LapTime)
                BestLap = CurrentLap;
            lapsInfo.Add(new LapInfo(sessionInfo.SessionTime, DriverInfo.CompletedLaps + 1));
            ComputePace();
        }

        private void UpdateInPitsProperty()
        {
            if (!InPits && DriverInfo.InPits)
            {
                InPits = true;
                PitCount++;
            }
            if (InPits && !DriverInfo.InPits)
                InPits = false;
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
            for(int i = lapsInfo.Count -2; i>=0 && totalPaceLaps <= 3; i--)
            {
                LapInfo lap = lapsInfo[i];
                if (!lap.Valid)
                    continue;
                pace = pace.Add(lap.LapTime);
                totalPaceLaps++;
            }
            if (totalPaceLaps == 0)
                Pace = new TimeSpan(0);
            else
                Pace = new TimeSpan(pace.Ticks / totalPaceLaps);
                
        }

        public LapInfo CurrentLap
        {
            get
            {
                if (lapsInfo.Count == 0)
                    return null;
                return lapsInfo.Last();
            }
        }

        public string CurrentLapProgressTime
        {
            get
            {
                if (CurrentLap == null)
                    return "";
                if (!CurrentLap.Valid)
                    return "Lap Invalid";
                TimeSpan progress = CurrentLap.LapProgressTime;            
                return FormatTimeSpan(progress);
            }
        }

        public LapInfo LastCompletedLap
        {
            get
            {
                if (lapsInfo.Count < 2)
                    return null;
                for (int i = lapsInfo.Count - 2; i >= 0; i--)
                {
                    if (lapsInfo[i].Valid)
                        return lapsInfo[i];
                }
                return null;
            }
        }

        public string LastLapTime
        {
            get
            {
                LapInfo lastCompletedLap = LastCompletedLap;
                if (lastCompletedLap != null)
                    return FormatTimeSpan(lastCompletedLap.LapTime);
                else
                    return "N/A";
            }
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            //String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            //return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }

        public static Driver FromModel(DriverInfo modelDriverInfo)
        {
            return new Driver(modelDriverInfo);
        }
    }
}
