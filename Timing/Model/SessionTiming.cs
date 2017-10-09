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

        private int paceLaps;
        private LapInfo bestSessionLap;
        public event EventHandler<BestLapChangedArgs> BestLapChangedEvent;
        public SessionInfo.SessionTypeEnum SessionType { get; private set; }
        
        private SessionTiming()
        {
            paceLaps = 4;
        }

        public Dictionary<string, Driver> Drivers { get; private set; } 
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

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet)
        {
            Dictionary<string, Driver> drivers = new Dictionary<string, Driver>();
            SessionTiming timing = new SessionTiming();
            //Driver[] drivers = Array.ConvertAll(dataSet.DriversInfo, s => Driver.FromModel(s));
            Array.ForEach(dataSet.DriversInfo, s => {
                String name = s.DriverName;
                int count = 1;
                while (drivers.Keys.Contains(name))
                {
                    name = s.DriverName + " dup" + count;
                    count++;
                }
                Driver newDriver = Driver.FromModel(s, timing);
                newDriver.PaceLaps = timing.paceLaps;
                drivers.Add(name, newDriver);
                });
            timing.Drivers = drivers;            
            return timing;
        }

        public LapInfo BestSessionLap { get => bestSessionLap; }
        public string BestLapFormatted { get => bestSessionLap != null ? Driver.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }
        

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
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
        private void UpdateDriver(DriverInfo modelInfo, Driver timingInfo, SimulatorDataSet set)
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
