namespace SecondMonitor.Timing.Controllers
{
    using System;
    using System.ComponentModel;
    using Contracts.TrackMap;
    using DataModel.TrackMap;
    using SessionTiming;
    using SessionTiming.ViewModel;
    using Settings.ViewModel;
    using SimdataManagement;
    using TrackMap;

    public class MapManagementController : IMapManagementController
    {
        private readonly MapsLoader _mapsLoader;
        private string _lastUnknownMap;
        private DisplaySettingsViewModel _displaySettingsViewModel;
        private SessionTiming _sessionSessionTiming;

        public event EventHandler<MapEventArgs> NewMapAvailable;
        public event EventHandler<MapEventArgs> MapRemoved;

        public MapManagementController(DisplaySettingsViewModel displaySettingsViewModel, TrackMapFromTelemetryFactory trackMapFromTelemetryFactory, MapsLoader mapsLoader)
        {
            _mapsLoader = mapsLoader;
            DisplaySettingsViewModel = displaySettingsViewModel;
            TrackMapFromTelemetryFactory = trackMapFromTelemetryFactory;
        }

        public TrackMapFromTelemetryFactory TrackMapFromTelemetryFactory { get; }

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsViewModel;
            set
            {
                UnsubscribeDisplaySettings();
                _displaySettingsViewModel = value;
                SubscribeDisplaySettings();
            }

        }

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
            if (string.IsNullOrWhiteSpace(_lastUnknownMap) || !e.Lap.Driver.IsPlayer || !e.Lap.Valid || e.Lap.LapTelemetryInfo?.TimedTelemetrySnapshots == null)
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
            SaveMap(e.Lap.Driver.Session.LastSet.Source, formattedTrackName, newTrack);
        }

        public bool TryGetMap(string simulator, string trackName, string layoutName, out TrackMapDto trackMapDto)
        {
            string formattedTrackName = FormatTrackName(trackName, layoutName);
            if (!_mapsLoader.TryLoadMap(simulator, formattedTrackName, out trackMapDto))
            {
                _lastUnknownMap = formattedTrackName;
                return false;
            }

            if (trackMapDto.TrackGeometry.ExporterVersion == TrackMapFromTelemetryFactory.ExporterVersion)
            {
                return true;
            }

            _lastUnknownMap = formattedTrackName;
            return false;
        }

        public void RemoveMap(string simulator, string trackName, string layoutName)
        {
            if (!TryGetMap(simulator, trackName, layoutName, out TrackMapDto trackMapDto))
            {
                return;
            }

            _mapsLoader.RemoveMap(simulator, FormatTrackName(trackName, layoutName));
            MapRemoved?.Invoke(this, new MapEventArgs(trackMapDto));
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
            _mapsLoader.SaveMap(simulator, formattedTrackName, trackMapDto);
            NewMapAvailable?.Invoke(this, new MapEventArgs(trackMapDto));
        }

        private void SubscribeDisplaySettings()
        {
            if (_displaySettingsViewModel != null)
            {
                _displaySettingsViewModel.PropertyChanged += OnDisplaySettingsChanged;
            }
        }

        private void OnDisplaySettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO: Implement
        }

        private void UnsubscribeDisplaySettings()
        {
            if (_displaySettingsViewModel != null)
            {
                _displaySettingsViewModel.PropertyChanged -= OnDisplaySettingsChanged;
            }
        }
    }
}