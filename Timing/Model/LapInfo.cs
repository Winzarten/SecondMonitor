using SecondMonitor.Timing.Model.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model
{
    public class LapInfo
    {
        private Driver driver;
        private TimeSpan lapEnd;
        private TimeSpan lapProgressTime;
        private TimeSpan lapTime;
        public LapInfo(TimeSpan startSeesionTine, int lapNumber, Driver driver)
        {
            Driver = driver;
            LapStart = startSeesionTine;
            lapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
        }
        public TimeSpan LapStart { get; private set; }
        public int LapNumber { get; private set; }
        public bool Valid { get; set; }
        public Driver Driver { get => driver; private set => driver = value; } 

        public void FinishLap(TimeSpan sessionTime)
        {
            lapEnd = sessionTime;
            lapTime = LapEnd.Subtract(LapStart);
        }
        public void Tick(TimeSpan sessionTime)
        {
            lapProgressTime = sessionTime.Subtract(LapStart);
        }

        public TimeSpan LapEnd { get => lapEnd; }
        public TimeSpan LapTime { get => lapTime; }
        public TimeSpan LapProgressTime { get => lapProgressTime; }
    }
}
