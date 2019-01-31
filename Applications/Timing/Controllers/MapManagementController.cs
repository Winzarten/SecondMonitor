namespace SecondMonitor.Timing.Controllers
{
    using System;
    using Contracts.TrackMap;
    using DataModel.TrackMap;
    using NLog;
    using SessionTiming;
    using SessionTiming.ViewModel;
    using SimdataManagement;

    public class MapManagementController : IMapManagementController
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MapsLoader _mapsLoader;
        private readonly ITrackDtoManipulator _trackDtoManipulator;
        private string _lastUnknownMap;
        private SessionTiming _sessionSessionTiming;

        public event EventHandler<MapEventArgs> NewMapAvailable;
        public event EventHandler<MapEventArgs> MapRemoved;

        public MapManagementController(TrackMapFromTelemetryFactory trackMapFromTelemetryFactory, MapsLoader mapsLoader, ITrackDtoManipulator trackDtoManipulator)
        {
            _mapsLoader = mapsLoader;
            _trackDtoManipulator = trackDtoManipulator;
            TrackMapFromTelemetryFactory = trackMapFromTelemetryFactory;
        }

        public TrackMapFromTelemetryFactory TrackMapFromTelemetryFactory { get; }

        public SessionTiming SessionTiming
        {
            get => _sessionSessionTiming;
            set
            {
                UnsubscribeSessionTiming();
                _sessionSessionTiming = value;
                SubscribeSessionTiming();
            }

        }

        public TimeSpan MapPointsInterval
        {
            get => TrackMapFromTelemetryFactory.MapsPointsInterval;
            set => TrackMapFromTelemetryFactory.MapsPointsInterval = value;

        }

        private void UnsubscribeSessionTiming()
        {
            if (_sessionSessionTiming == null)
            {
                return;
            }

            _sessionSessionTiming.LapCompleted -= OnLapCompleted;
        }

        private void SubscribeSessionTiming()
        {
            if (_sessionSessionTiming == null)
            {
                return;
            }

            _sessionSessionTiming.LapCompleted += OnLapCompleted;
        }

        private void OnLapCompleted(object sender, LapEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(_lastUnknownMap) || !e.Lap.Driver.IsPlayer || !e.Lap.Valid || e.Lap.LapTelemetryInfo?.TimedTelemetrySnapshots == null || e.Lap.LapTelemetryInfo.TimedTelemetrySnapshots.Snapshots.Count <= 0)
            {
                return;
            }

            if (e.Lap.Driver.Session.LastSet.SimulatorSourceInfo.WorldPositionInvalid)
            {
                return;
            }

            string formattedTrackName = FormatTrackName(e.Lap.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackName, e.Lap.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackLayoutName);

            if (formattedTrackName != _lastUnknownMap)
            {
                return;
            }

            TrackGeometryDto newTrackGeometryDto = TrackMapFromTelemetryFactory.BuildTrackGeometryDto(e.Lap.LapTelemetryInfo.TimedTelemetrySnapshots);


            TrackMapDto newTrack = new TrackMapDto()
            {
                TrackName = e.Lap.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackName,
                LayoutName = e.Lap.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackLayoutName,
                TrackGeometry = newTrackGeometryDto,
                SimulatorSource = e.Lap.Driver.Session.LastSet.Source,
            };

            Logger.Info($"Notified on Lap Completed on unknown map: {formattedTrackName}, Saving");
            SaveMap(e.Lap.Driver.Session.LastSet.Source, formattedTrackName, newTrack);
        }

        public bool TryGetMap(string simulator, string trackName, string layoutName, out TrackMapDto trackMapDto)
        {
            string formattedTrackName = FormatTrackName(trackName, layoutName);
            if (!_mapsLoader.TryLoadMap(simulator, formattedTrackName, out trackMapDto))
            {
                Logger.Info($"Trying to get map: {formattedTrackName}, Map unknown");
                _lastUnknownMap = formattedTrackName;
                return false;
            }

            if (trackMapDto.TrackGeometry.ExporterVersion == TrackMapFromTelemetryFactory.ExporterVersion)
            {
                Logger.Info($"Trying to get map: {formattedTrackName}, Map Loaded.");
                return true;
            }
            Logger.Info($"Trying to get map: {formattedTrackName}, Map Loaded, but outdated version.");
            _lastUnknownMap = formattedTrackName;
            return false;
        }

        public void RemoveMap(string simulator, string trackName, string layoutName)
        {
            if (!TryGetMap(simulator, trackName, layoutName, out TrackMapDto trackMapDto))
            {
                return;
            }
            Logger.Info($"Trying to remove map: Simulator {simulator}, Track Name '{trackName}', Layout Name: '{layoutName}'");
            _mapsLoader.RemoveMap(simulator, FormatTrackName(trackName, layoutName));
            MapRemoved?.Invoke(this, new MapEventArgs(trackMapDto));
        }

        public TrackMapDto RotateMapRight(string simulator, string trackName, string layoutName)
        {
            if (!TryGetMap(simulator, trackName, layoutName, out TrackMapDto trackMapDto))
            {
                throw new ArgumentException($"Unknown Map: {simulator} - {trackName} - {layoutName}");
            }
            trackMapDto = _trackDtoManipulator.RotateRight(trackMapDto);
            Logger.Info($"Trying to rotate map right: Simulator {simulator}, Track Name '{trackName}', Layout Name: '{layoutName}'");
            SaveMap(simulator, trackName, layoutName, trackMapDto);
            return trackMapDto;
        }

        public TrackMapDto RotateMapLeft(string simulator, string trackName, string layoutName)
        {
            if (!TryGetMap(simulator, trackName, layoutName, out TrackMapDto trackMapDto))
            {
                throw new ArgumentException($"Unknown Map: {simulator} - {trackName} - {layoutName}");
            }

            trackMapDto = _trackDtoManipulator.RotateLeft(trackMapDto);
            Logger.Info($"Trying to rotate left: Simulator {simulator}, Track Name '{trackName}', Layout Name: '{layoutName}'");
            SaveMap(simulator, trackName, layoutName, trackMapDto);
            return trackMapDto;
        }

        private static string FormatTrackName(string trackName, string layoutName)
        {
           return string.IsNullOrEmpty(layoutName) ? trackName :  $"{trackName}_{layoutName}";
        }

        public void SaveMap(string simulator, string trackName, string layoutName, TrackMapDto trackMapDto)
        {
            SaveMap(simulator, FormatTrackName(trackName, layoutName), trackMapDto);
        }

        private void SaveMap(string simulator, string formattedTrackName, TrackMapDto trackMapDto)
        {
            Logger.Info($"Trying to save map: Simulator {simulator}, Track Name '{formattedTrackName}'");
            _mapsLoader.SaveMap(simulator, formattedTrackName, trackMapDto);
            _lastUnknownMap = string.Empty;
            NewMapAvailable?.Invoke(this, new MapEventArgs(trackMapDto));
        }
    }
}