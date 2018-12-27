namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.MapView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using WindowsControls.WPF;
    using WindowsControls.WPF.DriverPosition;
    using Controllers.Synchronization;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot.Drivers;
    using DataModel.Telemetry;
    using DataModel.TrackMap;
    using Factory;
    using IOC;
    using NLog;
    using SecondMonitor.ViewModels;
    using SimdataManagement;
    using TelemetryManagement.DTO;

    public class MapViewViewModel : AbstractViewModel, IMapViewViewModel, IPositionCircleInformationProvider
    {

        private const int PedalStep = 5;

        private readonly IViewModelFactory _viewModelFactory;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceDictionary _commonResources;
        private FullMapControl _situationOverviewControl;
        private ILapColorSynchronization _lapColorSynchronization;
        private readonly Dictionary<string, ILapCustomPathsCollection> _lapsPaths;

        public MapViewViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _lapsPaths = new Dictionary<string, ILapCustomPathsCollection>();
            _commonResources = new ResourceDictionary
            {
                Source = new Uri(
                    @"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        public FullMapControl SituationOverviewControl
        {
            get => _situationOverviewControl;
            set
            {
                _situationOverviewControl = value;
                NotifyPropertyChanged();
            }
        }

        public ILapColorSynchronization LapColorSynchronization
        {
            get => _lapColorSynchronization;
            set
            {
                Unsubscribe();
                _lapColorSynchronization = value;
                Subscribe();
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

        public async Task AddPathsForLap(LapTelemetryDto lapTelemetry, TrackMapDto trackMapDto)
        {

            ILapCustomPathsCollection geometryCollection = GetOrCreateLapPathCollection(lapTelemetry.LapSummary.Id);
            if (!geometryCollection.FullyInitialized)
            {
                await InitializeGeometryCollection(geometryCollection, lapTelemetry, trackMapDto);
            }

            geometryCollection.GetAllPaths().ForEach(SituationOverviewControl.AddCustomPath);
        }

        private async Task InitializeGeometryCollection(ILapCustomPathsCollection geometryCollection, LapTelemetryDto lapTelemetry, TrackMapDto trackMapDto)
        {
            string baseGeometry = TrackMapFromTelemetryFactory.GetGeometry(lapTelemetry.TimedTelemetrySnapshots.ToList(), false, trackMapDto.TrackGeometry.XCoef, trackMapDto.TrackGeometry.YCoef, trackMapDto.TrackGeometry.IsSwappedAxis);
            Path geometryPath = new Path { Data = Geometry.Parse(baseGeometry), StrokeThickness = 0.5, Stroke = new SolidColorBrush() };
            if (!LapColorSynchronization.TryGetColorForLap(lapTelemetry.LapSummary.Id, out Color color))
            {
                color = Colors.DarkBlue;
            }
            geometryPath.Stroke = new SolidColorBrush(color);

            geometryCollection.BaseLapPath = geometryPath;

            string[] brakeGeometries = new string[0];
            string[] throttleGeometries = new string[0];
            string[] clutchGeometries = new string[0];
            string shiftPointsPath = string.Empty;

            Task brakeTask = Task.Run(() => brakeGeometries = GetGeometriesPathForBrakes(lapTelemetry, trackMapDto));
            Task throttleTask = Task.Run(() => throttleGeometries = GetGeometriesPathPedalInput(lapTelemetry, trackMapDto, x => x.ThrottlePedalPosition));
            Task clutchTask = Task.Run(() => clutchGeometries = GetGeometriesPathPedalInput(lapTelemetry, trackMapDto, x => x.ClutchPedalPosition));
            Task shiftPointsTask = Task.Run(() => shiftPointsPath = GetShiftPointsGeometry(lapTelemetry, trackMapDto));

            await Task.WhenAll(brakeTask, throttleTask, clutchTask, shiftPointsTask);
            //start from one, no reason to include "no brakes" geometry
            for (int i = 1; i < brakeGeometries.Length; i++)
            {
                string brakeGeometry = brakeGeometries[i];
                if (string.IsNullOrEmpty(brakeGeometry))
                {
                    continue;
                }
                Path brakePath = new Path { Data = Geometry.Parse(brakeGeometry), StrokeThickness = 5.0, Stroke = (i * PedalStep) > 99 ? Brushes.Red : Brushes.Orange, Opacity = (i * PedalStep) / 100.0 };
                geometryCollection.AddBrakingPath(brakePath, brakePath.Opacity);
            }

            for (int i = 1; i < throttleGeometries.Length; i++)
            {
                string throttleGeometry = throttleGeometries[i];
                if (string.IsNullOrEmpty(throttleGeometry))
                {
                    continue;
                }
                Path brakePath = new Path { Data = Geometry.Parse(throttleGeometry), StrokeThickness = 4.0, Stroke = (i * PedalStep) > 99 ? Brushes.Green : Brushes.LimeGreen, Opacity = (i * PedalStep) / 100.0 };
                geometryCollection.AddThrottlePath(brakePath, brakePath.Opacity);
            }

            for (int i = 1; i < clutchGeometries.Length; i++)
            {
                string clutchGeometry = clutchGeometries[i];
                if (string.IsNullOrEmpty(clutchGeometry))
                {
                    continue;
                }
                Path brakePath = new Path { Data = Geometry.Parse(clutchGeometry), StrokeThickness = 3.0, Stroke = Brushes.Yellow, Opacity = (i * PedalStep) / 100.0 };
                geometryCollection.AddClutchPath(brakePath, brakePath.Opacity);
            }

            Path shiftPointPath = new Path { Data = Geometry.Parse(shiftPointsPath), StrokeThickness = 5.0, Stroke = new SolidColorBrush(color)};
            geometryCollection.ShiftPointsPath = shiftPointPath;

            geometryCollection.FullyInitialized = true;
        }

        private string[] GetGeometriesPathForBrakes(LapTelemetryDto lapTelemetryDto, TrackMapDto trackMapDto)
        {
            return GetGeometriesPathPedalInput(lapTelemetryDto, trackMapDto, x => x.BrakePedalPosition);
        }

        private string[] GetGeometriesPathPedalInput(LapTelemetryDto lapTelemetryDto, TrackMapDto trackMapDto, Func<InputInfo, double> pedalValueFunc)
        {
            string[] geometries = new string[100 / PedalStep + 1];
            for (int i = 0; i < geometries.Length; i++)
            {
                geometries[i] = string.Empty;
            }

            int previousBrakeBand = -1;
            List<TimedTelemetrySnapshot> currentSnapShot = new List<TimedTelemetrySnapshot>();
            foreach (TimedTelemetrySnapshot timedTelemetrySnapshot in lapTelemetryDto.TimedTelemetrySnapshots)
            {
                int currentBrakeBand = ((int)(pedalValueFunc(timedTelemetrySnapshot.InputInfo) * 100)) / PedalStep;
                if (previousBrakeBand != -1 && previousBrakeBand != currentBrakeBand)
                {
                    currentSnapShot.Add(timedTelemetrySnapshot);
                    geometries[previousBrakeBand] += $" {TrackMapFromTelemetryFactory.GetGeometry(currentSnapShot, false, trackMapDto.TrackGeometry.XCoef, trackMapDto.TrackGeometry.YCoef, trackMapDto.TrackGeometry.IsSwappedAxis)}";
                    currentSnapShot.Clear();
                }
                currentSnapShot.Add(timedTelemetrySnapshot);
                previousBrakeBand = currentBrakeBand;
            }

            geometries[previousBrakeBand] += $" {TrackMapFromTelemetryFactory.GetGeometry(currentSnapShot, false, trackMapDto.TrackGeometry.XCoef, trackMapDto.TrackGeometry.YCoef, trackMapDto.TrackGeometry.IsSwappedAxis)}";
            currentSnapShot.Clear();

            return geometries;
        }

        private string GetShiftPointsGeometry(LapTelemetryDto lapTelemetryDto, TrackMapDto trackMapDto)
        {
            StringBuilder sb = new StringBuilder();
            List<TimedTelemetrySnapshot> shiftPoints = new List<TimedTelemetrySnapshot>();
            string currentGear = string.Empty;
            foreach (TimedTelemetrySnapshot timedTelemetrySnapshot in lapTelemetryDto.TimedTelemetrySnapshots)
            {
                if (!string.IsNullOrEmpty(currentGear) && currentGear != "N" && currentGear != timedTelemetrySnapshot.PlayerData.CarInfo.CurrentGear)
                {
                    shiftPoints.Add(timedTelemetrySnapshot);
                }

                currentGear = timedTelemetrySnapshot.PlayerData.CarInfo.CurrentGear;
            }

            TrackMapFromTelemetryFactory.ExtractWorldPoints(shiftPoints, trackMapDto.TrackGeometry.XCoef, trackMapDto.TrackGeometry.YCoef, trackMapDto.TrackGeometry.IsSwappedAxis).ForEach(x => sb.Append($"M{x.X-1},{x.Y-1} a2,2 0 1,0 0,-1 Z "));
            return sb.ToString();
        }

        public void RemovePathsForLap(LapSummaryDto lapSummaryDto)
        {
            ILapCustomPathsCollection geometryCollection = GetOrCreateLapPathCollection(lapSummaryDto.Id);
            geometryCollection.GetAllPaths().ForEach(SituationOverviewControl.RemoveCustomPath);
        }

        public void Dispose()
        {
            Subscribe();
        }

        protected ILapCustomPathsCollection GetOrCreateLapPathCollection(string id)
        {
            if (_lapsPaths.TryGetValue(id, out ILapCustomPathsCollection lapCustomPathsCollection))
            {
                return lapCustomPathsCollection;
            }

            ILapCustomPathsCollection newLapCustomPathsCollection = TaKernel.Instance.Get<ILapCustomPathsCollection>();
            _lapsPaths[id] = newLapCustomPathsCollection;
            return newLapCustomPathsCollection;

        }

        private void Unsubscribe()
        {
            if (_lapColorSynchronization == null)
            {
                return;
            }
            _lapColorSynchronization.LapColorChanged -= LapColorSynchronizationOnLapColorChanged;
        }

        private void Subscribe()
        {
            if (_lapColorSynchronization == null)
            {
                return;
            }
            _lapColorSynchronization.LapColorChanged += LapColorSynchronizationOnLapColorChanged;
        }

        private void LapColorSynchronizationOnLapColorChanged(object sender, LapColorArgs e)
        {
            if (SituationOverviewControl==null)
            {
                return;
            }
            ILapCustomPathsCollection lapPaths = GetOrCreateLapPathCollection(e.LapId);
            if (lapPaths.BaseLapPath != null)
            {
                lapPaths.BaseLapPath.Stroke = new SolidColorBrush(e.Color);
            }

            if (lapPaths.ShiftPointsPath != null)
            {
                lapPaths.ShiftPointsPath.Stroke = new SolidColorBrush(e.Color);
            }
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
            if (LapColorSynchronization == null)
            {
                outlineBrush = null;
                return false;
            }

            if (LapColorSynchronization.TryGetColorForLap(driverInfo.DriverName, out Color lapColor))
            {
                outlineBrush = new SolidColorBrush(lapColor);
                return true;
            }

            outlineBrush = null;
            return false;
        }
    }
}