namespace SecondMonitor.Timing.SessionTiming.Drivers.ModelView
{
    using System;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Drivers;

    public class LapInfo
    {
        public LapInfo(TimeSpan startSessionTime, int lapNumber, DriverTiming driver)
        {
            Driver = driver;
            LapStart = startSessionTime;
            LapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = false;
            PitLap = false;
        }

        public LapInfo(TimeSpan startSessionTime, int lapNumber, DriverTiming driver, bool firstLap)
        {
            Driver = driver;
            LapStart = startSessionTime;
            LapProgressTime = new TimeSpan(0, 0, 0);
            LapNumber = lapNumber;
            Valid = true;
            FirstLap = firstLap;
            PitLap = false;
        }

        public TimeSpan LapStart { get; }

        public int LapNumber { get; private set; }

        public bool Valid { get; set; }

        public DriverTiming Driver { get; }

        public bool FirstLap { get; }

        public bool InvalidBySim { get; set; }

        public bool PitLap { get; set; }

        public bool Completed { get; private set; }

        public void FinishLap(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            if (!dataSet.SimulatorSourceInfo.HasLapTimeInformation)
            {
                LapEnd = dataSet.SessionInfo.SessionTime;
            }
            else
            {
                LapEnd = LapStart.Add(TimeSpan.FromSeconds(driverInfo.Timing.LastLapTime));
            }

            LapTime = LapEnd.Subtract(LapStart);
            Completed = true;
        }

        public void Tick(SimulatorDataSet dataSet, DriverInfo driverInfo)
        {
            LapProgressTime = dataSet.SessionInfo.SessionTime.Subtract(LapStart);

            // Let 5 seconds for the source data noise, when lap count might not be properly updated at instance creation
            if (LapProgressTime.TotalSeconds < 5 && LapNumber != driverInfo.CompletedLaps + 1)
            {
                LapNumber = driverInfo.CompletedLaps + 1;
            }
        }

        public TimeSpan LapEnd { get; private set; }

        public TimeSpan LapTime { get; private set; }

        public TimeSpan LapProgressTime { get; private set; }
    }
}
