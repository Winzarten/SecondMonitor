namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System;
    using System.Threading.Tasks;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using Settings;
    using Synchronization;
    using Synchronization.Graphs;
    using ViewModels;
    using ViewModels.GraphPanel;

    public abstract class AbstractGraphPanelController : IGraphPanelController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ILapColorSynchronization _lapColorSynchronization;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IGraphViewSynchronization _graphViewSynchronization;

        protected AbstractGraphPanelController(IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization, ISettingsProvider settingsProvider, IGraphViewSynchronization graphViewSynchronization)
        {
            MainWindowViewModel = mainWindowViewModel;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _lapColorSynchronization = lapColorSynchronization;
            _settingsProvider = settingsProvider;
            _graphViewSynchronization = graphViewSynchronization;
        }

        public abstract bool IsLetPanel { get; }

        protected abstract IGraphViewModel[] Graphs { get; }
        protected IMainWindowViewModel MainWindowViewModel { get; }

        public Task StartControllerAsync()
        {
            Graphs.ForEach(InitializeViewModel);
            Subscribe();
            RefreshViewModels();
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            Unsubscribe();
            Graphs.ForEach(x => x.Dispose());
            return Task.CompletedTask;
        }

        private void InitializeViewModel(IGraphViewModel graphViewModel)
        {
            graphViewModel.GraphViewSynchronization = _graphViewSynchronization;
            graphViewModel.LapColorSynchronization = _lapColorSynchronization;
            graphViewModel.SuspensionDistanceUnits = _settingsProvider.DisplaySettingsViewModel.DistanceUnitsVerySmall;
            graphViewModel.DistanceUnits = _settingsProvider.DisplaySettingsViewModel.DistanceUnitsSmall;
            graphViewModel.VelocityUnits = _settingsProvider.DisplaySettingsViewModel.VelocityUnits;
            graphViewModel.TemperatureUnits = _settingsProvider.DisplaySettingsViewModel.TemperatureUnits;
            graphViewModel.PressureUnits = _settingsProvider.DisplaySettingsViewModel.PressureUnits;
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
            _telemetryViewsSynchronization.NewSessionLoaded += TelemetryViewsSynchronizationOnNewSessionLoaded;
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
        }

        private void TelemetryViewsSynchronizationOnNewSessionLoaded(object sender, TelemetrySessionArgs e)
        {
            Distance trackDistance = Distance.FromMeters(e.SessionInfoDto.LayoutLength);
            Graphs.ForEach(x => x.TrackDistance = trackDistance );
        }

        private void TelemetryViewsSynchronizationOnSyncTelemetryView(object sender, TelemetrySnapshotArgs e)
        {
            if (e.LapSummaryDto == null)
            {
                return;
            }

            Distance distance = Distance.FromMeters(e.TelemetrySnapshot.PlayerData.LapDistance);
            Graphs.ForEach(x => x.UpdateLapDistance(e.LapSummaryDto.Id, distance));

        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            Graphs.ForEach(x => x.RemoveLapTelemetry(e.LapSummary));
        }
        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            Graphs.ForEach(x => x.AddLapTelemetry(e.LapTelemetry));
        }

        private void Unsubscribe()
        {
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.NewSessionLoaded -= TelemetryViewsSynchronizationOnNewSessionLoaded;
        }

        protected abstract void RefreshViewModels();
    }
}