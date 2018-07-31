using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Snapshot;
using SecondMonitor.DataModel.Snapshot.Drivers;
using SecondMonitor.Timing.Presentation.ViewModel;
using SecondMonitor.Timing.Properties;
using SecondMonitor.Timing.SessionTiming.Drivers;
using SecondMonitor.Timing.SessionTiming.Drivers.ModelView;
using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
using SecondMonitor.WindowsControls.WPF;

namespace SecondMonitor.Timing.SessionTiming.ViewModel
{
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

        private readonly List<SectorTiming> _sector1Times = new List<SectorTiming>();

        private readonly List<SectorTiming> _sector2Times = new List<SectorTiming>();

        private readonly List<SectorTiming> _sector3Times = new List<SectorTiming>();

        public event EventHandler<DriverListModificationEventArgs> DriverAdded;

        public event EventHandler<DriverListModificationEventArgs> DriverRemoved;

        public double TotalSessionLength { get; private set; }

        private LapInfo _bestSessionLap;

        public LapInfo BestSessionLap
        {
            get => _bestSessionLap;
            set
            {
                _bestSessionLap = value;
                OnPropertyChanged();
            }
        }

        public DriverTimingModelView Player { get; private set; }

        public DriverTimingModelView Leader { get; private set; }

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

        public SectorTiming BestSector1 => _sector1Times.Any() ? _sector1Times.First() : null;

        public SectorTiming BestSector2 => _sector2Times.Any() ? _sector2Times.First() : null;

        public SectorTiming BestSector3 => _sector3Times.Any() ? _sector3Times.First() : null;

        public int SessionCompletedPerMiles
        {
            get
            {
                if (LastSet != null && LastSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
                {
                    return (int)(((LastSet.LeaderInfo.CompletedLaps
                                   + LastSet.LeaderInfo.LapDistance / LastSet.SessionInfo.TrackInfo.LayoutLength)
                                  / LastSet.SessionInfo.TotalNumberOfLaps) * 1000);
                }

                if (LastSet != null && LastSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
                {
                    return (int)(1000 - (LastSet.SessionInfo.SessionTimeRemaining / TotalSessionLength) * 1000);
                }

                return 0;
            }
        }

        public static SessionTiming FromSimulatorData(SimulatorDataSet dataSet, bool invalidateFirstLap, TimingDataViewModel timingDataViewModel)
        {
            Dictionary<string, DriverTimingModelView> drivers = new Dictionary<string, DriverTimingModelView>();
            SessionTiming timing = new SessionTiming(timingDataViewModel);
            timing.SessionType = dataSet.SessionInfo.SessionType;

            Array.ForEach(dataSet.DriversInfo, s =>
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
                DriverTimingModelView newDriverTimingModelView = new DriverTimingModelView(newDriver);
                drivers.Add(name, newDriverTimingModelView);
                if (newDriver.DriverInfo.IsPlayer)
                {
                    timing.Player = newDriverTimingModelView;
                }
            });
            timing.Drivers = drivers;
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
            {
                timing.TotalSessionLength = dataSet.SessionInfo.SessionTimeRemaining;
            }
            return timing;
        }

        private void DriverOnLapCompleted(object sender, DriverTiming.LapEventArgs lapEventArgs)
        {
            if (!lapEventArgs.Lap.Valid)
            {
                return;
            }
            if (BestSessionLap == null || BestSessionLap.LapTime > lapEventArgs.Lap.LapTime && lapEventArgs.Lap.LapTime != TimeSpan.Zero)
            {
                BestSessionLap = lapEventArgs.Lap;
            }
        }

        private void OnSectorCompletedEvent(object sender, LapInfo.SectorCompletedArgs e)
        {
            SectorTiming completedSector = e.SectorTiming;
            switch (completedSector.SectorNumber)
            {
                case 1:
                    if ((BestSector1 == null || BestSector1 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        _sector1Times.Insert(0, completedSector);
                        if (_sector1Times.Count > 50)
                        {
                            _sector1Times.RemoveAt(_sector1Times.Count - 1);
                        }
                        OnPropertyChanged(nameof(BestSector1));
                    }
                    break;
                case 2:
                    if ((BestSector2 == null || BestSector2 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        _sector2Times.Insert(0, completedSector);
                        if (_sector2Times.Count > 50)
                        {
                            _sector2Times.RemoveAt(_sector2Times.Count - 1);
                        }
                        OnPropertyChanged(nameof(BestSector2));
                    }
                    break;
                case 3:
                    if ((BestSector3 == null || BestSector3 > completedSector) && completedSector.Duration != TimeSpan.Zero)
                    {
                        _sector3Times.Insert(0, completedSector);
                        if (_sector3Times.Count > 50)
                        {
                            _sector3Times.RemoveAt(_sector3Times.Count - 1);
                        }
                        OnPropertyChanged(nameof(BestSector3));
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
            DriverTimingModelView newDriverTimingModelView = new DriverTimingModelView(newDriver);
            Drivers.Add(newDriver.Name, newDriverTimingModelView);
            RaiseDriverAddedEvent(newDriverTimingModelView);
        }

        private void LapInvalidatedHandler(object sender, DriverTiming.LapEventArgs e)
        {
            if (_sector1Times.Contains(e.Lap.Sector1))
            {
                _sector1Times.Remove(e.Lap.Sector1);
                OnPropertyChanged(nameof(BestSector1));
            }

            if (_sector2Times.Contains(e.Lap.Sector2))
            {
                _sector2Times.Remove(e.Lap.Sector2);
                OnPropertyChanged(nameof(BestSector2));
            }

            if (_sector3Times.Contains(e.Lap.Sector3))
            {
                _sector3Times.Remove(e.Lap.Sector3);
                OnPropertyChanged(nameof(BestSector3));
            }
        }

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

        private void UpdateDriver(DriverInfo modelInfo, DriverTimingModelView driverTimingModelView, SimulatorDataSet set)
        {
            DriverTiming timingInfo = driverTimingModelView.DriverTiming;
            timingInfo.DriverInfo = modelInfo;
            if (timingInfo.UpdateLaps(set) && timingInfo.LastCompletedLap != null && timingInfo.LastCompletedLap.LapTime != TimeSpan.Zero && (_bestSessionLap == null || timingInfo.LastCompletedLap.LapTime < _bestSessionLap.LapTime))
            {
                BestSessionLap = timingInfo.LastCompletedLap;
            }

            if (timingInfo.Position == 1)
            {
                Leader = driverTimingModelView;
            }

            driverTimingModelView.RefreshProperties();
        }

        public IEnumerator GetEnumerator()
        {
            return Drivers.Values.GetEnumerator();
        }

        public void RaiseDriverAddedEvent(DriverTimingModelView driver)
        {
            DriverAdded?.Invoke(this, new DriverListModificationEventArgs(driver));
        }

        public void RaiseDriverRemovedEvent(DriverTimingModelView driver)
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

            return !Drivers?[driver.DriverName].DriverTiming.CurrentLap?.Valid ?? false;
        }
    }
}
