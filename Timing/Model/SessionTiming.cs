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

        public class BestLapChangedArgs : EventArgs
        {            
            public BestLapChangedArgs(LapInfo lap)
            {
                Lap = lap;
            }
            public LapInfo Lap { get; set; }
                
        }
        private LapInfo bestSessionLap;
        public event EventHandler<BestLapChangedArgs> BestLapChangedEvent;
        
        private SessionTiming()
        {
            
        }

        public Dictionary<string, Driver> Drivers { get; private set; }

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
                    
                drivers.Add(name, Driver.FromModel(s));
                });
            timing.Drivers = drivers;            
            return timing;
        }

        public string BestLapFormatted { get => bestSessionLap != null ? Driver.FormatTimeSpan(bestSessionLap.LapTime) : "Best Session Lap"; }
        

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
            UpdateDrivers(dataSet);
        }

        private void UpdateDrivers(SimulatorDataSet dataSet)
        {
            Array.ForEach(dataSet.DriversInfo, s => UpdateDriver(s, Drivers[s.DriverName], dataSet));
        }
        private void UpdateDriver(DriverInfo modelInfo, Driver timingInfo, SimulatorDataSet set)
        {
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
