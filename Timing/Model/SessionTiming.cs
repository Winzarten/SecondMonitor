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

        public float TotalSessionLength { get; private set; }

        private int paceLaps;
        private LapInfo bestSessionLap;
        public DriverTiming Player { get; private set; }
        public TimeSpan SessionTime { get; private set; }
        public event EventHandler<BestLapChangedArgs> BestLapChangedEvent;
        public SessionInfo.SessionTypeEnum SessionType { get; private set; }
        public bool DisplayBindTimeRelative { get; set; }
        private SimulatorDataSet lastSet;
        
        private SessionTiming()
        {
            paceLaps = 4;
            DisplayBindTimeRelative = false;
        }

        public Dictionary<string, DriverTiming> Drivers { get; private set; } 
        public int PaceLaps
        {
            get
            {
                return paceLaps;
            }
            set
            {
                paceLaps = value;
                foreach (var driver in Drivers.Values)
                    driver.PaceLaps = value;
            }
        }

        public int SessionCompletedPercentage
        {
            get
            {
                if (lastSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Laps)
                    return (int)(((lastSet.LeaderInfo.CompletedLaps + lastSet.LeaderInfo.LapDistance / lastSet.SessionInfo.LayoutLength) / lastSet.SessionInfo.TotalNumberOfLaps) * 1000);
                if (lastSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                    return (int)(1000-(lastSet.SessionInfo.SessionTimeRemaining / TotalSessionLength ) * 1000);
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
                int count = 1;
                while (drivers.Keys.Contains(name))
                {
                    name = s.DriverName + " dup" + count;
                    count++;
                }
                DriverTiming newDriver = DriverTiming.FromModel(s, timing, invalidateFirstLap);
                newDriver.PaceLaps = timing.paceLaps;
                drivers.Add(name, newDriver);
                if (newDriver.DriverInfo.IsPlayer)
                    timing.Player = newDriver;
                    });
            timing.Drivers = drivers;
            if (dataSet.SessionInfo.SessionLengthType == SessionInfo.SessionLengthTypeEnum.Time)
                timing.TotalSessionLength = dataSet.SessionInfo.SessionTimeRemaining;            
            return timing;
        }

        public LapInfo BestSessionLap { get => bestSessionLap; }
        public string BestLapFormatted { get => bestSessionLap != null ? DriverTiming.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }
        

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
            lastSet = dataSet;
            SessionTime = dataSet.SessionInfo.SessionTime;
            SessionType = dataSet.SessionInfo.SessionType;
            UpdateDrivers(dataSet);
        }

        private void UpdateDrivers(SimulatorDataSet dataSet)
        {
            try
            {
                Array.ForEach(dataSet.DriversInfo, s => UpdateDriver(s, Drivers[s.DriverName], dataSet));
            }catch(KeyNotFoundException ex)
            {
                throw new DriverNotFoundException("Driver nout found", ex);

            }
        }
        private void UpdateDriver(DriverInfo modelInfo, DriverTiming timingInfo, SimulatorDataSet set)
        {
            if (set.SessionInfo.SessionPhase == SessionInfo.SessionPhaseEnum.Checkered)
                return;
            timingInfo.DriverInfo = modelInfo;
            if(timingInfo.UpdateLaps(set) && (bestSessionLap==null || timingInfo.LastCompletedLap.LapTime < bestSessionLap.LapTime))
            {
                bestSessionLap = timingInfo.LastCompletedLap;
                RaiseBestLapChangedEvent(bestSessionLap);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }

        public void RaiseBestLapChangedEvent(LapInfo lapInfo)
        {
            BestLapChangedEvent?.Invoke(this, new BestLapChangedArgs(lapInfo));
        }
    }
}
