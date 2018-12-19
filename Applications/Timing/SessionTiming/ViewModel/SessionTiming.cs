﻿using System.Windows.Media;

namespace SecondMonitor.Timing.SessionTiming.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
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
    using NLog;
    using SimdataManagement.DriverPresentation;

    public class SessionTiming : DependencyObject, IPositionCircleInformationProvider, IEnumerable, INotifyPropertyChanged
    {
        private readonly DriverPresentationsManager _driverPresentationsManager;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class DriverNotFoundException : Exception
        {
            public DriverNotFoundException(string message) : base(message)
            {

            }

            public DriverNotFoundException(string message, Exception cause) : base(message, cause)
            {

            }
        }

        public event EventHandler<LapEventArgs> LapCompleted;
        public event EventHandler<DriverListModificationEventArgs> DriverAdded;
        public event EventHandler<DriverListModificationEventArgs> DriverRemoved;

        public double TotalSessionLength { get; private set; }

        public TimeSpan SessionStarTime { get; private set; }

        private DateTime _nextUpdateTime = DateTime.Now;

        private LapInfo _bestSessionLap;

        private SectorTiming _bestSector1;
        private SectorTiming _bestSector2;
        private SectorTiming _bestSector3;

        private CombinedLapPortionComparatorsViewModel _combinedLapPortionComparatorsViewModel;

        private SessionTiming(TimingDataViewModel timingDataViewModel, DriverPresentationsManager driverPresentationsManager)
        {
            _driverPresentationsManager = driverPresentationsManager;
            PaceLaps = 4;
            DisplayBindTimeRelative = false;
            TimingDataViewModel = timingDataViewModel;
        }

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

        public CombinedLapPortionComparatorsViewModel CombinedLapPortionComparator
        {
            get => _combinedLapPortionComparatorsViewModel;
            private set
            {
                _combinedLapPortionComparatorsViewModel = value;
                OnPropertyChanged();
            }
        }

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

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet, bool invalidateFirstLap, TimingDataViewModel timingDataViewModel, DriverPresentationsManager driverPresentationsManager)
        {
            Dictionary<string, DriverTimingViewModel> drivers = new Dictionary<string, DriverTimingViewModel>();
            Logger.Info($"New Seesion Started :{dataSet.SessionInfo.SessionType.ToString()}");
            SessionTiming timing = new SessionTiming(timingDataViewModel, driverPresentationsManager)
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
                DriverTimingViewModel newDriverTimingViewModel = new DriverTimingViewModel(newDriver, timingDataViewModel.DisplaySettingsViewModel,driverPresentationsManager);
                drivers.Add(name, newDriverTimingViewModel);
                Logger.Info($"Driver Added: {name}");
                if (newDriver.DriverInfo.IsPlayer)
                {
                    timing.Player = newDriverTimingViewModel;
                    timing.CombinedLapPortionComparator = new CombinedLapPortionComparatorsViewModel(newDriver);
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
            LapCompleted?.Invoke(this, lapEventArgs);

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
            if (TimingDataViewModel.GuiDispatcher != null && !TimingDataViewModel.GuiDispatcher.CheckAccess())
            {
                TimingDataViewModel.GuiDispatcher.Invoke(() => AddNewDriver(newDriverInfo));
                return;
            }

            if (Drivers.ContainsKey(newDriverInfo.DriverName))
            {
                if (Drivers[newDriverInfo.DriverName].DriverTiming.IsActive)
                {
                    return;
                }
                Drivers.Remove(newDriverInfo.DriverName);
            }
            Logger.Info($"Adding new driver: {newDriverInfo.DriverName}");
            DriverTiming newDriver = DriverTiming.FromModel(newDriverInfo, this, SessionType != SessionType.Race);
            newDriver.SectorCompletedEvent += OnSectorCompletedEvent;
            newDriver.LapInvalidated += LapInvalidatedHandler;
            newDriver.LapCompleted += DriverOnLapCompleted;
            DriverTimingViewModel newDriverTimingViewModel = new DriverTimingViewModel(newDriver, TimingDataViewModel.DisplaySettingsViewModel, _driverPresentationsManager);
            Drivers.Add(newDriver.Name, newDriverTimingViewModel);
            if (newDriver.IsPlayer)
            {
                CombinedLapPortionComparator.Driver = newDriver;
            }
            RaiseDriverAddedEvent(newDriverTimingViewModel);
            Logger.Info($"Added new driver");
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
                List<string> driversToRemove = Drivers.Keys.Where(s => !updatedDrivers.Contains(s) && Drivers[s].DriverTiming.IsActive).ToList();

                driversToRemove.ForEach(s =>
                {
                    Logger.Info($"Removing driver {Drivers[s].Name}");
                    RaiseDriverRemovedEvent(Drivers[s]);
                    Drivers[s].DriverTiming.IsActive = false;
                    Logger.Info($"Driver Removed");
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

        public bool IsDriverLastSectorGreen(DriverInfo driver, int sectorNumber)
        {
            if (driver == null || string.IsNullOrEmpty(driver.DriverName) || Drivers == null || !Drivers.ContainsKey(driver.DriverName))
            {
                return false;
            }

            switch (sectorNumber)
            {
                case 1:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector1PersonalBest;
                case 2:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector2PersonalBest;
                case 3:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector3PersonalBest;
            }

            return false;
        }

        public bool IsDriverLastSectorPurple(DriverInfo driver, int sectorNumber)
        {
            if (driver == null || string.IsNullOrEmpty(driver.DriverName) || Drivers == null || !Drivers.ContainsKey(driver.DriverName))
            {
                return false;
            }

            switch (sectorNumber)
            {
                case 1:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector1SessionBest;
                case 2:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector2SessionBest;
                case 3:
                    return Drivers != null && Drivers[driver.DriverName].IsLastSector3SessionBest;
            }

            return false;
        }

        public bool GetTryCustomOutline(DriverInfo driverInfo, out SolidColorBrush outlineBrush)
        {
            if (driverInfo == null || string.IsNullOrEmpty(driverInfo.DriverName) || Drivers == null || !Drivers.TryGetValue(driverInfo.DriverName, out DriverTimingViewModel driverTimingViewModel))
            {
                outlineBrush = null;
                return false;
            }

            outlineBrush = driverTimingViewModel.HasCustomOutline ? driverTimingViewModel.OutlineBrush : null;
            return driverTimingViewModel.HasCustomOutline;

        }
    }
}
