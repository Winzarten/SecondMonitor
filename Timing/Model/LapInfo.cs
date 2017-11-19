using SecondMonitor.Timing.Model.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.Timing.Model
{
    public class LapInfo
    {
        private DriverTiming driver;
        private TimeSpan lapEnd;
        private TimeSpan lapProgressTime;
        private TimeSpan lapTime;
        
        public LapInfo(TimeSpan startSeesionTine, int lapNumber, DriverTiming driver)
        {
            Driver = driver;
            LapStart = startSeesionTine;
            lapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = false;
            PitLap = false;
        }

        public LapInfo(TimeSpan startSeesionTine, int lapNumber, DriverTiming driver, bool firstLap)
        {
            Driver = driver;
            LapStart = startSeesionTine;
            lapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = firstLap;
            PitLap = false;
        }
        public TimeSpan LapStart { get; private set; }
        public int LapNumber { get; private set; }
        public bool Valid { get; set; }
        public DriverTiming Driver { get => driver; private set => driver = value; } 
        public bool FirstLap { get; private set; }
        public bool InvalidBySim { get; set; }
        public bool PitLap { get; set; }
        public bool Completed { get; private set; }

        public void FinishLap(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation)
            {
                lapEnd = dataSet.SessionInfo.SessionTime;
            }
            else
            {
                lapEnd = LapStart.Add(TimeSpan.FromSeconds(driverInfo.Timing.LastLapTime));
            }
            lapTime = LapEnd.Subtract(LapStart);
            Completed = true;
        }
        public void Tick(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            lapProgressTime = dataSet.SessionInfo.SessionTime.Subtract(LapStart);
            //Let 5 seconds for the source data noise, when lap count might not be properly updated at instance creation
            if (lapProgressTime.TotalSeconds < 5 && LapNumber != driverInfo.CompletedLaps + 1)
                LapNumber = driverInfo.CompletedLaps + 1;
        }

        public TimeSpan LapEnd { get => lapEnd; }
        public TimeSpan LapTime { get => lapTime; }
        public TimeSpan LapProgressTime { get => lapProgressTime; }
    }
}
