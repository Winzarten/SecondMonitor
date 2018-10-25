namespace SecondMonitor.Timing.SessionTiming.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;
    using SecondMonitor.Timing.Presentation.ViewModel;
    using Properties;
    using Drivers;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using WindowsControls.WPF;

    public class SessionTiming : DependencyObject, IPositionCircleInformationProvider, IEnumerable, INotifyPropertyChanged
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

        public event EventHandler<DriverListModificationEventArgs> DriverAdded;

        public event EventHandler<DriverListModificationEventArgs> DriverRemoved;

        public double TotalSessionLength { get; private set; }

        public TimeSpan SessionStarTime { get; private set; }

        private TimeSpan _nextUpdateTime = TimeSpan.Zero;

        private LapInfo _bestSessionLap;

        private SectorTiming _bestSector1;
        private SectorTiming _bestSector2;
        private SectorTiming _bestSector3;

        public LapInfo BestSessionLap
        {
            get => _bestSessionLap;
            set
            {
                _bestSessionLap = value;
                OnPropertyChanged();
            }
        }

        public DriverTimingViewModel Player { get; private set; }

        public DriverTimingViewModel Leader { get; private set; }

        public TimeSpan SessionTime { get; private set; }


        public SessionType SessionType { get; private set; }

        public bool DisplayBindTimeRelative
        {
            get;
            set;
        }

        public bool DisplayGapToPlayerRelative { get; set; }

        public TimingDataViewModel TimingDataViewModel { get; private set; }

        public SimulatorDataSet LastSet { get; private set; } = new SimulatorDataSet("None");


        public Dictionary<string, DriverTimingViewModel> Drivers { get; private set; }

        public int PaceLaps
        {
            get;
            set;
        }

        public SectorTiming BestSector1
        {
            get => _bestSector1;
            private set
            {
                _bestSector1 = value;
                OnPropertyChanged();
            }
        }

        public SectorTiming BestSector2
        {
            get => _bestSector2;
            private set
            {
                _bestSector2 = value;
                OnPropertyChanged();
            }
        }

        public SectorTiming BestSector3
        {
            get => _bestSector3;
            private set
            {
                _bestSector3 = value;
                OnPropertyChanged();
            }
        }

        public CombinedLapPortionComparatorsVM CombinedLapPortionComparators{ get; }

        public bool RetrieveAlsoInvalidLaps { get; set; }

        public int SessionCompletedPerMiles
        {
            get
            {
                if (LastSet != null && LastSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
                {
                    return (int)(((LastSet.LeaderInfo.CompletedLaps
                                   + LastSet.LeaderInfo.LapDistance / LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters)
                                  / LastSet.SessionInfo.TotalNumberOfLaps) * 1000);
                }

                if (LastSet != null && LastSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
                {
                    return (int)(1000 - (LastSet.SessionInfo.SessionTimeRemaining / TotalSessionLength) * 1000);
                }

                return 0;
            }
        }

        private SessionTiming(TimingDataViewModel timingDataViewModel)
        {
            PaceLaps = 4;
            DisplayBindTimeRelative = false;
            TimingDataViewModel = timingDataViewModel;
            CombinedLapPortionComparators = new CombinedLapPortionComparatorsVM(null);
        }

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet, bool invalidateFirstLap, TimingDataViewModel timingDataViewModel)
        {
            Dictionary<string, DriverTimingViewModel> drivers = new Dictionary<string, DriverTimingViewModel>();
            SessionTiming timing = new SessionTiming(timingDataViewModel)
                                       {
                                           SessionStarTime = dataSet.SessionInfo.SessionTime,
                                           SessionType = dataSet.SessionInfo.SessionType,
                                           RetrieveAlsoInvalidLaps = dataSet.SessionInfo.SessionType == SessionType.Race
                                       };

            Array.ForEach(
                dataSet.DriversInfo,
                s =>
                    {
                        var name = s.DriverName;
                if (drivers.Keys.Contains(name))
                {
                    return;
                }

                DriverTiming newDriver = DriverTiming.FromModel(s, timing, invalidateFirstLap);
                newDriver.SectorCompletedEvent += timing.OnSectorCompletedEvent;
                newDriver.LapInvalidated += timing.LapInvalidatedHandler;
                newDriver.LapCompleted += timing.DriverOnLapCompleted;
                DriverTimingViewModel newDriverTimingViewModel = new DriverTimingViewModel(newDriver);
                drivers.Add(name, newDriverTimingViewModel);
                if (newDriver.DriverInfo.IsPlayer)
                {
                    timing.Player = newDriverTimingViewModel;
                }
            });
            timing.Drivers = drivers;
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
            {
                timing.TotalSessionLength = dataSet.SessionInfo.SessionTimeRemaining;
            }
            return timing;
        }

        private void DriverOnLapCompleted(object sender, LapEventArgs lapEventArgs)
        {
            if (!lapEventArgs.Lap.Valid)
            {
                return;
            }

            if (BestSessionLap == null || (BestSessionLap.LapTime > lapEventArgs.Lap.LapTime && lapEventArgs.Lap.LapTime != TimeSpan.Zero))
            {
                BestSessionLap = lapEventArgs.Lap;
            }
        }

        private void OnSectorCompletedEvent(object sender, LapInfo.SectorCompletedArgs e)
        {
            SectorTiming completedSector = e.SectorTiming;
            if (!e.SectorTiming.Lap.Valid)
            {
                return;
            }

            switch (completedSector.SectorNumber)
            {
                case 1:
                    if ((BestSector1 == null || BestSector1 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector1 = completedSector;
                    }
                    break;
                case 2:
                    if ((BestSector2 == null || BestSector2 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector2 = completedSector;
                    }
                    break;
                case 3:
                    if ((BestSector3 == null || BestSector3 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        BestSector3 = completedSector;
                    }
                    break;
            }
        }

        private void AddNewDriver(DriverInfo newDriverInfo)
        {
            if (Drivers.ContainsKey(newDriverInfo.DriverName))
            {
                if (Drivers[newDriverInfo.DriverName].DriverTiming.IsActive)
                {
                    return;
                }
                Drivers.Remove(newDriverInfo.DriverName);
            }
            DriverTiming newDriver = DriverTiming.FromModel(newDriverInfo, this, SessionType != SessionType.Race);
            newDriver.SectorCompletedEvent += OnSectorCompletedEvent;
            newDriver.LapInvalidated += LapInvalidatedHandler;
            newDriver.LapCompleted += DriverOnLapCompleted;
            DriverTimingViewModel newDriverTimingViewModel = new DriverTimingViewModel(newDriver);
            Drivers.Add(newDriver.Name, newDriverTimingViewModel);
            RaiseDriverAddedEvent(newDriverTimingViewModel);
        }

        private void LapInvalidatedHandler(object sender, LapEventArgs e)
        {
            if (BestSector1 == e.Lap.Sector1)
            {
                BestSector1 = FindBestSector(d => d.BestSector1);
            }

            if (BestSector2 == e.Lap.Sector2)
            {
                BestSector2 = FindBestSector(d => d.BestSector2);
            }

            if (BestSector3 == e.Lap.Sector3)
            {
                BestSector3 = FindBestSector(d => d.BestSector3);
            }
        }

        private SectorTiming FindBestSector(Func<DriverTiming, SectorTiming> sectorTimeFunc)
        {
            return Drivers.Values.Where(d => d.DriverTiming.IsActive).Select(x => sectorTimeFunc(x.DriverTiming)).Where( s => s != null)
                .OrderBy(s => s.Duration).FirstOrDefault();
        }

        public void UpdateTiming(SimulatorDataSet dataSet)
        {
            LastSet = dataSet;
            SessionTime = dataSet.SessionInfo.SessionTime - SessionStarTime;
            SessionType = dataSet.SessionInfo.SessionType;
            UpdateDrivers(dataSet);
        }

        private void UpdateDrivers(SimulatorDataSet dataSet)
        {
            try
            {
               HashSet<string> updatedDrivers = new HashSet<string>();
                Array.ForEach( dataSet.DriversInfo,
                    s =>
                        {
                            updatedDrivers.Add(s.DriverName);
                            if (Drivers.ContainsKey(s.DriverName) && Drivers[s.DriverName].DriverTiming.IsActive)
                            {
                                UpdateDriver(s, Drivers[s.DriverName], dataSet);

                                if (Drivers[s.DriverName].DriverTiming.IsPlayer)
                                {
                                    Player = Drivers[s.DriverName];
                                }
                            }
                            else
                            {
                                AddNewDriver(s);
                            }
                        });
                List<string> driversToRemove = new List<string>();
                foreach (var obsoleteDriverName in Drivers.Keys.Where(s => !updatedDrivers.Contains(s)))
                {
                    driversToRemove.Add(obsoleteDriverName);
                }

                driversToRemove.ForEach(s =>
                {
                    RaiseDriverRemovedEvent(Drivers[s]);
                    Drivers[s].DriverTiming.IsActive = false;
                });


            }
            catch (KeyNotFoundException ex)
            {
                throw new DriverNotFoundException("Driver not found", ex);

            }
        }

        private void UpdateDriver(DriverInfo modelInfo, DriverTimingViewModel driverTimingViewModel, SimulatorDataSet set)
        {
            DriverTiming timingInfo = driverTimingViewModel.DriverTiming;
            timingInfo.DriverInfo = modelInfo;
            if (timingInfo.UpdateLaps(set) && timingInfo.LastCompletedLap != null && timingInfo.LastCompletedLap.LapTime != TimeSpan.Zero && (_bestSessionLap == null || timingInfo.LastCompletedLap.LapTime < _bestSessionLap.LapTime))
            {
                BestSessionLap = timingInfo.LastCompletedLap;
            }

            if (timingInfo.Position == 1)
            {
                Leader = driverTimingViewModel;
            }

            if (driverTimingViewModel.DriverTiming.IsPlayer && driverTimingViewModel.DriverTiming.CurrentLap
                != CombinedLapPortionComparators.PlayerLap)
            {
                CombinedLapPortionComparators.PlayerLap = driverTimingViewModel.DriverTiming.CurrentLap;
            }

            driverTimingViewModel.RefreshProperties();
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }

        public void RaiseDriverAddedEvent(DriverTimingViewModel driver)
        {
            DriverAdded?.Invoke(this, new DriverListModificationEventArgs(driver));
        }

        public void RaiseDriverRemovedEvent(DriverTimingViewModel driver)
        {
            DriverRemoved?.Invoke(this, new DriverListModificationEventArgs(driver));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDriverOnValidLap(DriverInfo driver)
        {
            if (!Drivers?.ContainsKey(driver.DriverName) ?? false)
            {
                return false;
            }

            return Drivers?[driver.DriverName].DriverTiming.CurrentLap?.Valid ?? false;
        }
    }
}
