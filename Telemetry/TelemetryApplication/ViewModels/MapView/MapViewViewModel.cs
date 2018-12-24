namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using WindowsControls.WPF;
    using WindowsControls.WPF.DriverPosition;
    using DataModel.Snapshot.Drivers;
    using DataModel.TrackMap;
    using NLog;
    using SecondMonitor.ViewModels;

    public class MapViewViewModel : AbstractViewModel, IMapViewViewModel, IPositionCircleInformationProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceDictionary _commonResources;
        private ISituationOverviewControl _situationOverviewControl;

        public MapViewViewModel()
        {
            _commonResources = new ResourceDictionary
            {
                Source = new Uri(
                    @"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        public ISituationOverviewControl SituationOverviewControl
        {
            get => _situationOverviewControl;
            set
            {
                _situationOverviewControl = value;
                NotifyPropertyChanged();
            }
        }

        public void RemoveDriver(IDriverInfo driverInfo)
        {
            _situationOverviewControl.RemoveDrivers(driverInfo);
        }

        public void UpdateDrivers(params IDriverInfo[] driversInfo)
        {
            _situationOverviewControl.UpdateDrivers(null, driversInfo);
        }

        public void LoadTrack(ITrackMap trackMapDto)
        {
            SituationOverviewControl = InitializeFullMap(trackMapDto);
        }

        private FullMapControl InitializeFullMap(ITrackMap trackMapDto)
        {
            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.Invoke(() => InitializeFullMap(trackMapDto));
            }

            Logger.Info("Initializing Full Map Control");

            return new FullMapControl(trackMapDto)
            {
                PositionCircleInformationProvider = this,

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

                GreenSectorBrush = (SolidColorBrush)_commonResources["Green01Brush"],
                PurpleSectorBrush = (SolidColorBrush)_commonResources["PurpleTimingBrush"],
                YellowSectorBrush = (SolidColorBrush)_commonResources["YellowSectorBrush"],

                PlayerOutLineBrush = (SolidColorBrush)_commonResources["PlayerOutLineColor"],

                AnimateDriversPos = false,
                DataContext = this,
                AutoScaleDriverControls = false,
                KeepMapRatio = true,
                EnableSidePanel = false
            };
        }

        public bool IsDriverOnValidLap(IDriverInfo driver) => true;

        public bool IsDriverLastSectorGreen(IDriverInfo driver, int sectorNumber) => false;

        public bool IsDriverLastSectorPurple(IDriverInfo driver, int sectorNumber) => false;

        public bool GetTryCustomOutline(IDriverInfo driverInfo, out SolidColorBrush outlineBrush)
        {
            outlineBrush = null;
            return false;
        }
    }
}