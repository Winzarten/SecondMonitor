namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.MapView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel.TrackMap;
    using Settings;
    using SimdataManagement;
    using Synchronization;
    using TelemetryManagement.DTO;
    using ViewModels.MapView;

    public class MapViewController : IMapViewController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ILapColorSynchronization _lapColorSynchronization;
        private readonly MapsLoader _mapsLoader;
        private TrackMapDto _lastMap;
        private bool _mapAvailable;
        private readonly Dictionary<string, MapViewDriverInfoFacade> _fakeDrivers;

        public MapViewController(ISettingsProvider settingsProvider, IMapsLoaderFactory mapsLoaderFactory, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization)
        {
            _fakeDrivers = new Dictionary<string, MapViewDriverInfoFacade>();
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _lapColorSynchronization = lapColorSynchronization;
            _mapsLoader = mapsLoaderFactory.Create(settingsProvider.MapRepositoryPath);
        }

        public IMapViewViewModel MapViewViewModel { get; set; }

        public void StartController()
        {
            MapViewViewModel.LapColorSynchronization = _lapColorSynchronization;
            Subscribe();
        }

        public void StopController()
        {
            UnSubscribe();
            MapViewViewModel?.Dispose();
        }

        private void InitializeViewModel(SessionInfoDto sessionInfo)
        {
            _fakeDrivers.Clear();
            string formattedTrackName = FormatTrackName(sessionInfo.TrackName, sessionInfo.LayoutName);
            _mapAvailable = _mapsLoader.TryLoadMap(sessionInfo.Simulator, formattedTrackName, out _lastMap);
            if (_mapAvailable)
            {
                MapViewViewModel.LoadTrack(_lastMap);
            }
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded += TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronization_LapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
        }

        private void UnSubscribe()
        {
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronization_LapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.SyncTelemetryView -= TelemetryViewsSynchronizationOnSyncTelemetryView;
        }

        private void TelemetryViewsSynchronization_LapLoaded(object sender, LapTelemetryArgs e)
        {
            if (!_mapAvailable)
            {
                return;
            }

            MapViewViewModel.AddPathsForLap(e.LapTelemetry, _lastMap);

        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            if (!_mapAvailable)
            {
                return;
            }

            MapViewViewModel.RemovePathsForLap(e.LapSummary);

            if (_fakeDrivers.TryGetValue(e.LapSummary.Id, out MapViewDriverInfoFacade fakeDriver))
            {
                MapViewViewModel.RemoveDriver(fakeDriver);
            }
        }

        private void TelemetryViewsSynchronizationOnSyncTelemetryView(object sender, TelemetrySnapshotArgs e)
        {
            if (!_mapAvailable)
            {
                return;
            }

            string driverId = e.LapSummaryDto.Id;
            if (!_fakeDrivers.TryGetValue(driverId, out MapViewDriverInfoFacade fakeDriver))
            {
                fakeDriver = new MapViewDriverInfoFacade(e.TelemetrySnapshot.PlayerData, e.LapSummaryDto.LapNumber, driverId);
                _fakeDrivers.Add(driverId,fakeDriver);
            }
            else
            {
                fakeDriver.ParentInfo = e.TelemetrySnapshot.PlayerData;
            }
            MapViewViewModel.UpdateDrivers(fakeDriver);
        }

        private async void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            await Task.Run(() => InitializeViewModel(e.SessionInfoDto));
        }

        private static string FormatTrackName(string trackName, string layoutName)
        {
            return string.IsNullOrEmpty(layoutName) ? trackName : $"{trackName}_{layoutName}";
        }

    }
}