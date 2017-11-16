using SecondMonitor.DataModel;
using SecondMonitor.DataModel.Drivers;
using SecondMonitor.Timing.Model.Drivers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model
{
    public class DriverListModificationEventArgs : EventArgs
    {

        public DriverListModificationEventArgs(DriverTiming data)
        {
            this.Data = data;
        }

        public DriverTiming Data
        {
            get;
            set;
        }
    }
    public class SessionTiming : IEnumerable
    {        
        public class DriverNotFoundException : Exception
        {
            public DriverNotFoundException(string message) : base(message)
            {

            }

            public DriverNotFoundException(string message, Exception cause) : base(message, cause)
            {

            }
        }

        public class BestLapChangedArgs : EventArgs
        {            
            public BestLapChangedArgs(LapInfo lap)
            {
                Lap = lap;
            }
            public LapInfo Lap { get; set; }
                
        }

        public event EventHandler<DriverListModificationEventArgs> DriverAdded;
        public event EventHandler<DriverListModificationEventArgs> DriverRemoved;

        public float TotalSessionLength { get; private set; }

        
        private LapInfo bestSessionLap;
        public Drivers.DriverTiming Player { get; private set; }
        public Drivers.DriverTiming Leader { get; private set; }
        public TimeSpan SessionTime { get; private set; }
        public event EventHandler<BestLapChangedArgs> BestLapChangedEvent;
        public SessionInfo.SessionTypeEnum SessionType { get; private set; }
        public bool DisplayBindTimeRelative { get; set; }
        public bool DisplayGapToPlayerRelative { get; set; }
        public SimulatorDataSet LastSet { get; private set; } = new SimulatorDataSet("None");
        
        private SessionTiming()
        {
            PaceLaps = 4;
            DisplayBindTimeRelative = false;
        }

        public Dictionary<string, DriverTiming> Drivers { get; private set; } 
        public int PaceLaps
        {
            get;
            set;
        }

        public int SessionCompletedPercentage
        {
            get
            {
                if (LastSet!=null && LastSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Laps)
                    return (int)(((LastSet.LeaderInfo.CompletedLaps + LastSet.LeaderInfo.LapDistance / LastSet.SessionInfo.LayoutLength) / LastSet.SessionInfo.TotalNumberOfLaps) * 1000);
                if (LastSet != null && LastSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                    return (int)(1000-(LastSet.SessionInfo.SessionTimeRemaining / TotalSessionLength ) * 1000);
                return 0;
            }
        }

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet, bool invalidateFirstLap)
        {
            Dictionary<string, DriverTiming> drivers = new Dictionary<string, DriverTiming>();
            SessionTiming timing = new SessionTiming();
            timing.SessionType = dataSet.SessionInfo.SessionType;
            //Driver[] drivers = Array.ConvertAll(dataSet.DriversInfo, s => Driver.FromModel(s));
            Array.ForEach(dataSet.DriversInfo, s => {
                String name = s.DriverName;
                if (drivers.Keys.Contains(name))
                {
                    return;
                }
                DriverTiming newDriver = DriverTiming.FromModel(s, timing, invalidateFirstLap);
                drivers.Add(name, newDriver);
                if (newDriver.DriverInfo.IsPlayer)
                    timing.Player = newDriver;
                    });
            timing.Drivers = drivers;
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                timing.TotalSessionLength = dataSet.SessionInfo.SessionTimeRemaining;            
            return timing;
        }

        private void AddNewDriver(DriverInfo newDriverInfo)
        {
            if (Drivers.ContainsKey(newDriverInfo.DriverName))
                return;
            DriverTiming newDriver = DriverTiming.FromModel(newDriverInfo, this, SessionType != SessionInfo.SessionTypeEnum.Race);
            Drivers.Add(newDriver.Name, newDriver);
            RaiseDriverAddedEvent(newDriver);
        }

        public LapInfo BestSessionLap { get => bestSessionLap; }
        public string BestLapFormatted { get => bestSessionLap != null ? DriverTiming.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }
        

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
            LastSet = dataSet;
            SessionTime = dataSet.SessionInfo.SessionTime;
            SessionType = dataSet.SessionInfo.SessionType;
            UpdateDrivers(dataSet);
        }

        private void UpdateDrivers(SimulatorDataSet dataSet)
        {
            try
            {
                HashSet<string> updatedDrivers = new HashSet<string>();
                Array.ForEach(dataSet.DriversInfo, s =>
                {
                    updatedDrivers.Add(s.DriverName);
                    if (Drivers.ContainsKey(s.DriverName))
                    {
                        UpdateDriver(s, Drivers[s.DriverName], dataSet);
                    }
                    else
                    {
                        AddNewDriver(s);
                    }
                });
                List<string> driversToRemove = new List<string>();
                foreach (var opsoliteDriverName in Drivers.Keys.Where(s => !updatedDrivers.Contains(s)))
                {
                    driversToRemove.Add(opsoliteDriverName);
                }
                {                    
                    driversToRemove.ForEach(s =>
                    {
                        RaiseDriverRemovedEvent(Drivers[s]);
                        Drivers.Remove(s);
                    });
                }

            }
            catch(KeyNotFoundException ex)
            {
                throw new DriverNotFoundException("Driver nout found", ex);

            }
        }
        private void UpdateDriver(DriverInfo modelInfo, DriverTiming timingInfo, SimulatorDataSet set)
        {
            timingInfo.DriverInfo = modelInfo;
            if(timingInfo.UpdateLaps(set) && (bestSessionLap==null || timingInfo.LastCompletedLap.LapTime < bestSessionLap.LapTime))
            {
                bestSessionLap = timingInfo.LastCompletedLap;
                RaiseBestLapChangedEvent(bestSessionLap);
            }
            if (timingInfo.Position == 1)
                Leader = timingInfo;
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }

        public void RaiseBestLapChangedEvent(LapInfo lapInfo)
        {
            BestLapChangedEvent?.Invoke(this, new BestLapChangedArgs(lapInfo));
        }

        public void RaiseDriverAddedEvent(DriverTiming driver)
        {
            DriverAdded?.Invoke(this, new DriverListModificationEventArgs(driver));
        }

        public void RaiseDriverRemovedEvent(DriverTiming driver)
        {
            DriverRemoved?.Invoke(this, new DriverListModificationEventArgs(driver));
        }
    }
}
