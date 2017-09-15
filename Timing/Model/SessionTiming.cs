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
        private SessionTiming()
        {

        }

        public Dictionary<string, Driver> Drivers { get; private set; }

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet)
        {
            Dictionary<string, Driver> drivers = new Dictionary<string, Driver>();
            SessionTiming timing = new SessionTiming();
            //Driver[] drivers = Array.ConvertAll(dataSet.DriversInfo, s => Driver.FromModel(s));
            Array.ForEach(dataSet.DriversInfo, s => drivers.Add(s.DriverName, Driver.FromModel(s)));
            timing.Drivers = drivers;            
            return timing;
        }

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
            UpdateDrivers(dataSet.DriversInfo);
        }

        private void UpdateDrivers(DriverInfo[] driversInfo)
        {
            Array.ForEach(driversInfo, s => UpdateDriver(s, Drivers[s.DriverName]));
        }
        private void UpdateDriver(DriverInfo modelInfo, Driver timingInfo)
        {
            timingInfo.Position = modelInfo.Position;
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }
    }
}
