namespace SecondMonitor.ViewModels.SituationOverview
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using WindowsControls.WPF;
    using WindowsControls.WPF.Commands;
    using WindowsControls.WPF.DriverPosition;
    using Contracts.TrackMap;
    using DataModel.TrackMap;
    using Properties;
    using Settings.ViewModel;
    using Timing.Controllers;

    public class SituationOverviewProvider : ISimulatorDataSetViewModel, INotifyPropertyChanged, IMapSidePanelViewModel
    {

        private readonly ResourceDictionary _commonResources;
        private ISituationOverviewControl _situationOverviewControl;
        private IPositionCircleInformationProvider _positionCircleInformationProvider;
        private IMapManagementController _mapManagementController;
        private (string trackName, string layoutName, string simName) _currentTrackTuple;
        private DisplaySettingsViewModel _displaySettingsViewModel;
        private bool _alwaysUseCircle;

        public SituationOverviewProvider(IPositionCircleInformationProvider positionCircleInformation, DisplaySettingsViewModel displaySettingsViewModel)
        {
            _commonResources = new ResourceDictionary
                                   {
                                       Source = new Uri(
                                           @"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml",
                                           UriKind.RelativeOrAbsolute)
                                   };
            _displaySettingsViewModel = DisplaySettingsViewModel;
            PositionCircleInformationProvider = positionCircleInformation;
        }

        public ICommand DeleteMapCommand  => new RelayCommand(RemoveCurrentMap);

        public ISituationOverviewControl SituationOverviewControl
        {
            get => _situationOverviewControl;
            private set
            {
                _situationOverviewControl = value;
                NotifyPropertyChanged();
            }
        }

        public IPositionCircleInformationProvider PositionCircleInformationProvider
        {
            get => _positionCircleInformationProvider;

            set
            {
                _positionCircleInformationProvider = value;
                if(SituationOverviewControl != null)
                {
                    SituationOverviewControl.PositionCircleInformationProvider = value;
                }
            }
        }

        public IMapManagementController MapManagementController
        {
            get => _mapManagementController;
            set
            {
                UnsubscribeMapManager();
                _mapManagementController = value;
                SubscribeMapManager();
            }
        }

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsViewModel;
            set
            {
                UnSubscribeDisplaySettings();
                _displaySettingsViewModel = value;
                ApplyDisplaySettings();
                SubscribeDisplaySettings();
            }
        }

        private bool AlwaysUseCircle
        {
            get => _alwaysUseCircle;
            set
            {
                if (_alwaysUseCircle == value)
                {
                    return;
                }

                _alwaysUseCircle = value;
                LoadCurrentMap();
            }
        }

        private void ApplyDisplaySettings()
        {
            if (_displaySettingsViewModel == null )
            {
                return;
            }

            AlwaysUseCircle = _displaySettingsViewModel.MapDisplaySettingsViewModel.AlwaysUseCirce;

            if (MapManagementController != null)
            {
                MapManagementController.MapPointsInterval = TimeSpan.FromMilliseconds(DisplaySettingsViewModel.MapDisplaySettingsViewModel.MapPointsInterval);
            }

            if (SituationOverviewControl == null)
            {
                return;
            }

            SituationOverviewControl.AnimateDriversPos = DisplaySettingsViewModel.AnimateDriversPosition;

            if (SituationOverviewControl is FullMapControl fullMapControl)
            {
                fullMapControl.AutoScaleDriverControls = DisplaySettingsViewModel.MapDisplaySettingsViewModel.AutoScaleDrivers;
                fullMapControl.KeepMapRatio = DisplaySettingsViewModel.MapDisplaySettingsViewModel.KeepMapRatio;
            }


        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (_currentTrackTuple.trackName != dataSet.SessionInfo.TrackInfo.TrackName || _currentTrackTuple.layoutName != dataSet.SessionInfo.TrackInfo.TrackLayoutName || _currentTrackTuple.simName != dataSet.Source)
            {
                _currentTrackTuple = (dataSet.SessionInfo.TrackInfo.TrackName, dataSet.SessionInfo.TrackInfo.TrackLayoutName, dataSet.Source);
                LoadCurrentMap();
            }

            SituationOverviewControl?.UpdateDrivers(dataSet, dataSet.DriversInfo);
        }

        public void Reset()
        {
            _situationOverviewControl?.RemoveAllDrivers();
        }

        public void RemoveDriver(DriverInfo driver)
        {
            _situationOverviewControl.RemoveDrivers(driver);
        }

        public void AddDriver(DriverInfo driver)
        {
            _situationOverviewControl.AddDrivers(driver);
        }

        private PositionCircleControl InitializePositionCircle()
        {
            return new PositionCircleControl
            {
                PositionCircleInformationProvider = PositionCircleInformationProvider,

                DriverBackgroundBrush = (SolidColorBrush)_commonResources["DriverBackgroundColor"],
                DriverForegroundBrush = (SolidColorBrush)_commonResources["DriverForegroundColor"],

                DriverPitsBackgroundBrush = (SolidColorBrush)_commonResources["DriverPitsBackgroundColor"],
                DriverPitsForegroundBrush = (SolidColorBrush)_commonResources["DriverPitsForegroundColor"],

                PlayerBackgroundBrush = (SolidColorBrush)_commonResources["PlayerBackgroundColor"],
                PlayerForegroundBrush = (SolidColorBrush)_commonResources["PlayerForegroundColor"],

                LappedDriverBackgroundBrush = (SolidColorBrush)_commonResources["TimingLappedBrush"],
                LappedDriverForegroundBrush = (SolidColorBrush)_commonResources["TimingLappedForegroundBrush"],

                LappingDriverBackgroundBrush = (SolidColorBrush)_commonResources["TimingLappingBrush"],
                LappingDriverForegroundBrush = (SolidColorBrush)_commonResources["TimingLappingForegroundBrush"],

                AnimateDriversPos = DisplaySettingsViewModel.AnimateDriversPosition
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SubscribeMapManager()
        {
            if (_mapManagementController == null)
            {
                return;
            }

            _mapManagementController.NewMapAvailable += OnNewMapAvailable;
            _mapManagementController.MapRemoved += OnMapRemoved;
        }

        private void UnsubscribeMapManager()
        {
            if (_mapManagementController == null)
            {
                return;
            }

            _mapManagementController.NewMapAvailable -= OnNewMapAvailable;
            _mapManagementController.MapRemoved -= OnMapRemoved;
        }

        private void SubscribeDisplaySettings()
        {
            if (_displaySettingsViewModel == null)
            {
                return;
            }

            _displaySettingsViewModel.PropertyChanged += OnDisplaySettingsChanged;
            _displaySettingsViewModel.MapDisplaySettingsViewModel.PropertyChanged += OnDisplaySettingsChanged;
        }

        private void UnSubscribeDisplaySettings()
        {
            if (_displaySettingsViewModel == null)
            {
                return;
            }

            _displaySettingsViewModel.PropertyChanged -= OnDisplaySettingsChanged;
            _displaySettingsViewModel.MapDisplaySettingsViewModel.PropertyChanged -= OnDisplaySettingsChanged;
        }

        private void OnDisplaySettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplyDisplaySettings();
        }


        private void LoadCurrentMap()
        {
            if (string.IsNullOrEmpty(_currentTrackTuple.trackName) || _mapManagementController == null)
            {
                return;
            }

            if (AlwaysUseCircle || !_mapManagementController.TryGetMap(_currentTrackTuple.simName, _currentTrackTuple.trackName, _currentTrackTuple.layoutName, out TrackMapDto trackMapDto))
            {
                SituationOverviewControl = InitializePositionCircle();
                if (!AlwaysUseCircle)
                {
                    SituationOverviewControl.AdditionalInformation = $"No Valid Map for {_currentTrackTuple.trackName}\nComplete one valid lap for full map";
                }
            }
            else
            {
                SituationOverviewControl = InitializeFullMap(trackMapDto);
            }
        }

        private FullMapControl InitializeFullMap(TrackMapDto trackMapDto)
        {
            return new FullMapControl(trackMapDto)
            {
                PositionCircleInformationProvider = PositionCircleInformationProvider,

                DriverBackgroundBrush = (SolidColorBrush) _commonResources["DriverBackgroundColor"],
                DriverForegroundBrush = (SolidColorBrush) _commonResources["DriverForegroundColor"],

                DriverPitsBackgroundBrush = (SolidColorBrush) _commonResources["DriverPitsBackgroundColor"],
                DriverPitsForegroundBrush = (SolidColorBrush) _commonResources["DriverPitsForegroundColor"],

                PlayerBackgroundBrush = (SolidColorBrush) _commonResources["PlayerBackgroundColor"],
                PlayerForegroundBrush = (SolidColorBrush) _commonResources["PlayerForegroundColor"],

                LappedDriverBackgroundBrush = (SolidColorBrush) _commonResources["TimingLappedBrush"],
                LappedDriverForegroundBrush = (SolidColorBrush) _commonResources["TimingLappedForegroundBrush"],

                LappingDriverBackgroundBrush = (SolidColorBrush) _commonResources["TimingLappingBrush"],
                LappingDriverForegroundBrush = (SolidColorBrush) _commonResources["TimingLappingForegroundBrush"],

                AnimateDriversPos = DisplaySettingsViewModel.AnimateDriversPosition,
                DataContext = this,
                AutoScaleDriverControls = DisplaySettingsViewModel.MapDisplaySettingsViewModel.AutoScaleDrivers,
                KeepMapRatio = DisplaySettingsViewModel.MapDisplaySettingsViewModel.KeepMapRatio,
            };
        }

        private void OnNewMapAvailable(object sender, MapEventArgs e)
        {
            if (_currentTrackTuple.simName == e.TrackMapDto.SimulatorSource && _currentTrackTuple.trackName == e.TrackMapDto.TrackName && _currentTrackTuple.layoutName == e.TrackMapDto.LayoutName)
            {
                SituationOverviewControl = InitializeFullMap(e.TrackMapDto);
            }
        }

        private void OnMapRemoved(object sender, MapEventArgs e)
        {
            if (_currentTrackTuple.simName == e.TrackMapDto.SimulatorSource && _currentTrackTuple.trackName == e.TrackMapDto.TrackName && _currentTrackTuple.layoutName == e.TrackMapDto.LayoutName)
            {
                LoadCurrentMap();
            }
        }

        private void RemoveCurrentMap()
        {
            _mapManagementController.RemoveMap(_currentTrackTuple.simName, _currentTrackTuple.trackName, _currentTrackTuple.layoutName);
        }
    }
}