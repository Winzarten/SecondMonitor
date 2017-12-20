namespace SecondMonitor.Timing.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Drivers;
    using SecondMonitor.Timing.DataHandler;
    using SecondMonitor.Timing.Model.Drivers;
    using SecondMonitor.Timing.Model.Drivers.ModelView;

    public class DriverListModificationEventArgs : EventArgs
    {

        public DriverListModificationEventArgs(DriverTimingModelView data)
        {
            Data = data;
        }

        public DriverTimingModelView Data
        {
            get;
            set;
        }
    }
    public class SessionTiming : DependencyObject, IEnumerable
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

        
        private LapInfo _bestSessionLap;
        public DriverTimingModelView Player { get; private set; }
        public DriverTimingModelView Leader { get; private set; }
        public TimeSpan SessionTime { get; private set; }
        public event EventHandler<BestLapChangedArgs> BestLapChangedEvent;
        public SessionInfo.SessionTypeEnum SessionType { get; private set; }

        public bool DisplayBindTimeRelative
        {
            get;
            set;
        }
        public bool DisplayGapToPlayerRelative { get; set; }
        public TimingDataViewModel TimingDataViewModel { get; private set; }
        public SimulatorDataSet LastSet { get; private set; } = new SimulatorDataSet("None");
        
        private SessionTiming(TimingDataViewModel timingDataViewModel)
        {
            PaceLaps = 4;
            DisplayBindTimeRelative = false;
            TimingDataViewModel = timingDataViewModel;
        }

        public Dictionary<string, DriverTimingModelView> Drivers { get; private set; } 
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

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet, bool invalidateFirstLap, TimingDataViewModel timingDataViewModel)
        {
            Dictionary<string, DriverTimingModelView> drivers = new Dictionary<string, DriverTimingModelView>();
            SessionTiming timing = new SessionTiming(timingDataViewModel);
            timing.SessionType = dataSet.SessionInfo.SessionType;
            //Driver[] drivers = Array.ConvertAll(dataSet.DriversInfo, s => Driver.FromModel(s));
            Array.ForEach(dataSet.DriversInfo, s => {
                String name = s.DriverName;
                if (drivers.Keys.Contains(name))
                {
                    return;
                }
                DriverTiming newDriver = DriverTiming.FromModel(s, timing, invalidateFirstLap);
                DriverTimingModelView  newDriverTimingModelView = new DriverTimingModelView(newDriver);
                drivers.Add(name, newDriverTimingModelView);
                if (newDriver.DriverInfo.IsPlayer)
                    timing.Player = newDriverTimingModelView;
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
            DriverTimingModelView newDriverTimingModelView = new DriverTimingModelView(newDriver);
            Drivers.Add(newDriver.Name, newDriverTimingModelView);
            RaiseDriverAddedEvent(newDriverTimingModelView);
        }

        public LapInfo BestSessionLap { get => _bestSessionLap; }
        public string BestLapFormatted { get => _bestSessionLap != null ? DriverTiming.FormatTimeSpan(_bestSessionLap.LapTime) : "Best Session Lap"; }
        

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
                        
                        if (Drivers[s.DriverName].DriverTiming.IsPlayer)
                            Player = Drivers[s.DriverName];
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
        private void UpdateDriver(DriverInfo modelInfo, DriverTimingModelView DriverTimingModelView, SimulatorDataSet set)
        {
            DriverTiming timingInfo = DriverTimingModelView.DriverTiming;
            timingInfo.DriverInfo = modelInfo;
            if(timingInfo.UpdateLaps(set) && timingInfo.LastCompletedLap != null && (_bestSessionLap==null || timingInfo.LastCompletedLap.LapTime < _bestSessionLap.LapTime))
            {
                _bestSessionLap = timingInfo.LastCompletedLap;
                RaiseBestLapChangedEvent(_bestSessionLap);
            }
            if (timingInfo.Position == 1)
                Leader = DriverTimingModelView;

            DriverTimingModelView.RefreshProperties();
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }

        public void RaiseBestLapChangedEvent(LapInfo lapInfo)
        {
            BestLapChangedEvent?.Invoke(this, new BestLapChangedArgs(lapInfo));
        }

        public void RaiseDriverAddedEvent(DriverTimingModelView driver)
        {
            DriverAdded?.Invoke(this, new DriverListModificationEventArgs(driver));
        }

        public void RaiseDriverRemovedEvent(DriverTimingModelView driver)
        {
            DriverRemoved?.Invoke(this, new DriverListModificationEventArgs(driver));
        }
    }
}
