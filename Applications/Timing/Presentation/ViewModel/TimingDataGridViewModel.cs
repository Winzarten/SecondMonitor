namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using WindowsControls.WPF;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using NLog;
    using SessionTiming.Drivers.Presentation.ViewModel;
    using SessionTiming.Drivers.ViewModel;
    using SimdataManagement.DriverPresentation;
    using ViewModels;
    using ViewModels.Settings.Model;
    using ViewModels.Settings.ViewModel;

    public class TimingDataGridViewModel : AbstractViewModel, IPositionCircleInformationProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DriverPresentationsManager _driverPresentationsManager;
        private readonly DisplaySettingsViewModel _displaySettingsViewModel;
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, DriverTiming> _driverNameTimingMap;
        private int _loadIndex;
        private readonly Stopwatch _refreshGapWatch;

        public TimingDataGridViewModel(DriverPresentationsManager driverPresentationsManager, DisplaySettingsViewModel displaySettingsViewModel )
        {
            _refreshGapWatch = Stopwatch.StartNew();
            _loadIndex = 0;
            _driverNameTimingMap = new Dictionary<string, DriverTiming>();
            _driverPresentationsManager = driverPresentationsManager;
            _displaySettingsViewModel = displaySettingsViewModel;
            DriversViewModels = new ObservableCollection<DriverTimingViewModel>();
        }

        public ObservableCollection<DriverTimingViewModel> DriversViewModels { get; }

        public DisplayModeEnum DriversOrdering { get; set; }

        public DriverTimingViewModel PlayerViewModel { get; set; }

        public void UpdateProperties(SimulatorDataSet dataSet)
        {
            if (_loadIndex > 0)
            {
                return;
            }
            lock (_lockObject)
            {
                List<DriverTiming> orderedTimings = (DriversOrdering == DisplayModeEnum.Absolute ? _driverNameTimingMap.Values.OrderBy(x => x.Position) : _driverNameTimingMap.Values.OrderBy(x => x.DistanceToPlayer)).ToList();
                for (int i = 0; i < orderedTimings.Count; i++)
                {
                    DriversViewModels[i].DriverTiming = orderedTimings[i];
                    if (DriversViewModels[i].IsPlayer)
                    {
                        PlayerViewModel = DriversViewModels[i];
                    }
                }
                DriversViewModels.ForEach(x => x.RefreshProperties());
                if (_refreshGapWatch.ElapsedMilliseconds < 500)
                {
                    return;
                }

                UpdateGapsSize(dataSet);
                _refreshGapWatch.Restart();
            }
        }

        private void UpdateGapsSize(SimulatorDataSet dataSet)
        {
            if (dataSet?.SessionInfo == null || dataSet.SessionInfo.SessionType != SessionType.Race || DriversOrdering != DisplayModeEnum.Relative)
            {
                return;
            }

            bool isVisualizationEnabled = _displaySettingsViewModel.IsGapVisualizationEnabled;
            double minimalGap = _displaySettingsViewModel.MinimalGapForVisualization;
            double heightPerSecond = _displaySettingsViewModel.GapHeightForOneSecond;
            double maximumGap = _displaySettingsViewModel.MaximumGapHeight;

            DriverTimingViewModel previousViewModel = null;
            foreach (DriverTimingViewModel driverTimingViewModel in DriversViewModels)
            {
                if (previousViewModel == null)
                {
                    previousViewModel = driverTimingViewModel;
                    continue;
                }

                double gapInSeconds = Math.Abs(driverTimingViewModel.GapToPlayer.TotalSeconds - previousViewModel.GapToPlayer.TotalSeconds) - minimalGap;
                if (!isVisualizationEnabled || gapInSeconds < 0 )
                {
                    previousViewModel.GapHeight = 0;
                    previousViewModel = driverTimingViewModel;
                    continue;
                }
                previousViewModel.GapHeight = Math.Min(gapInSeconds * heightPerSecond, maximumGap);
                previousViewModel = driverTimingViewModel;

            }
        }

        public void RemoveDriver(DriverTiming driver)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => RemoveDriver(driver));
                return;
            }

            lock (_lockObject)
            {
                DriverTimingViewModel toRemove = DriversViewModels.FirstOrDefault(x => x.DriverTiming == driver);
                if (toRemove == null)
                {
                    return;
                }

                _driverNameTimingMap.Remove(driver.Name);
                DriversViewModels.Remove(toRemove);
            }

        }

        private void RemoveAllDrivers()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(RemoveAllDrivers);
                return;
            }

            lock (_lockObject)
            {
                _driverNameTimingMap.Clear();
                DriversViewModels.Clear();
            };
        }

        public void AddDriver(DriverTiming driver)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => AddDriver(driver));
                return;
            }

            DriverTimingViewModel newViewModel = new DriverTimingViewModel(driver, _driverPresentationsManager);
            lock (_lockObject)
            {
                //If possible, rebind - do not create new
                if (_driverNameTimingMap.ContainsKey(driver.Name))
                {
                    _driverNameTimingMap[driver.Name] = driver;
                    DriversViewModels.First(x => x.Name == driver.Name).DriverTiming = driver;
                    return;
                }
                _driverNameTimingMap[driver.Name] = driver;
                DriversViewModels.Add(newViewModel);
            }
        }

        public void MatchDriversList(List<DriverTiming> drivers)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => MatchDriversList(drivers));
                return;
            }

            lock (_lockObject)
            {
                IEnumerable<DriverTiming> driversToRemove = _driverNameTimingMap.Values.Where(x => drivers.FirstOrDefault(y => y.Name == x.Name) == null).ToList();
                IEnumerable<DriverTiming> driversToAdd = drivers.Where(x => !_driverNameTimingMap.ContainsKey(x.Name)).ToList();
                IEnumerable<DriverTiming> driversToRebind = drivers.Where(x => _driverNameTimingMap.ContainsKey(x.Name)).ToList();

                driversToRemove.ForEach(RemoveDriver);
                AddDrivers(driversToAdd);

                foreach (DriverTiming driverToRebind in driversToRebind)
                {
                    _driverNameTimingMap[driverToRebind.Name] = driverToRebind;
                    DriversViewModels.First(x => x.Name == driverToRebind.Name).DriverTiming = driverToRebind;
                }
            }
        }

        private void AddDrivers(IEnumerable<DriverTiming> drivers)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => AddDrivers(drivers));
                return;
            }

            try
            {
                _loadIndex++;
                Logger.Info("Add Drivers Called");
                List<DriverTimingViewModel> newViewModels = drivers.Select(x => new DriverTimingViewModel(x, _driverPresentationsManager)).ToList();

                foreach (DriverTimingViewModel driverTimingViewModel in newViewModels)
                {
                    lock (_lockObject)
                    {
                        if (_driverNameTimingMap.ContainsKey(driverTimingViewModel.Name))
                        {
                            continue;
                        }
                        _driverNameTimingMap[driverTimingViewModel.Name] = driverTimingViewModel.DriverTiming;
                        DriversViewModels.Add(driverTimingViewModel);
                    }
                }

                _loadIndex--;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info("Add Drivers Completed");
        }

        public bool IsDriverOnValidLap(IDriverInfo driver)
        {
            if (driver?.DriverName == null)
            {
                return false;
            }

            lock (_lockObject)
            {
                if (_driverNameTimingMap.TryGetValue(driver.DriverName, out DriverTiming timing))
                {
                    return timing.CurrentLap?.Valid ?? false;
                }
            }

            return false;
        }

        public bool IsDriverLastSectorGreen(IDriverInfo driver, int sectorNumber)
        {
            if (driver?.DriverName == null)
            {
                return false;
            }

            DriverTiming timing;
            lock (_lockObject)
            {
                if (!_driverNameTimingMap.TryGetValue(driver.DriverName, out timing))
                {
                    return false;
                }
            }

            switch (sectorNumber)
            {
                case 1:
                    return timing.IsLastSector1PersonalBest;
                case 2:
                    return timing.IsLastSector2PersonalBest;
                case 3:
                    return timing.IsLastSector3PersonalBest;
                default:
                    return false;
            }
        }

        public bool IsDriverLastSectorPurple(IDriverInfo driver, int sectorNumber)
        {
            if (driver?.DriverName == null)
            {
                return false;
            }
            DriverTiming timing;
            lock (_lockObject)
            {
                if (!_driverNameTimingMap.TryGetValue(driver.DriverName, out timing))
                {
                    return false;
                }
            }

            switch (sectorNumber)
            {
                case 1:
                    return timing.IsLastSector1SessionBest;
                case 2:
                    return timing.IsLastSector2SessionBest;
                case 3:
                    return timing.IsLastSector3SessionBest;
                default:
                    return false;
            }
        }

        public bool GetTryCustomOutline(IDriverInfo driverInfo, out SolidColorBrush outlineBrush)
        {
            if (driverInfo?.DriverName == null)
            {
                outlineBrush = null;
                return false;
            }
            DriverTimingViewModel viewModel;
            lock (DriversViewModels)
            {
                viewModel = DriversViewModels.FirstOrDefault(x => x.Name == driverInfo.DriverName);
            }

            outlineBrush = viewModel?.OutlineBrush ?? default(SolidColorBrush);
            return viewModel?.HasCustomOutline ?? false;
        }
    }
}