namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataModel.Extensions;
    using SecondMonitor.ViewModels.Settings;
    using Settings;
    using Settings.DTO;
    using Synchronization;
    using Synchronization.Graphs;
    using TelemetryManagement.DTO;
    using ViewModels;
    using ViewModels.GraphPanel;

    public abstract class AbstractGraphPanelController : IGraphPanelController
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ILapColorSynchronization _lapColorSynchronization;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IGraphViewSynchronization _graphViewSynchronization;
        private readonly ITelemetrySettingsRepository _telemetrySettingsRepository;
        private readonly List<LapTelemetryDto> _loadedLaps;
        private TelemetrySettingsDto _telemetrySettingsDto;

        protected AbstractGraphPanelController(IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization, ISettingsProvider settingsProvider,
            IGraphViewSynchronization graphViewSynchronization, ITelemetrySettingsRepository telemetrySettingsRepository)
        {
            MainWindowViewModel = mainWindowViewModel;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _lapColorSynchronization = lapColorSynchronization;
            _settingsProvider = settingsProvider;
            _graphViewSynchronization = graphViewSynchronization;
            _telemetrySettingsRepository = telemetrySettingsRepository;
            _loadedLaps = new List<LapTelemetryDto>();
        }

        public abstract bool IsLetPanel { get; }

        protected abstract IGraphViewModel[] Graphs { get; set; }
        protected IMainWindowViewModel MainWindowViewModel { get; }

        public Task StartControllerAsync()
        {
            Subscribe();
            _telemetrySettingsDto = _telemetrySettingsRepository.LoadOrCreateNew();
            ReloadGraphCollection();
            Graphs.ForEach(InitializeViewModel);
            return Task.CompletedTask;
        }

        public Task StopControllerAsync()
        {
            Unsubscribe();
            Graphs.ForEach(x => x.Dispose());
            _loadedLaps.Clear();
            return Task.CompletedTask;
        }

        private void InitializeViewModel(IGraphViewModel graphViewModel)
        {
            graphViewModel.XAxisKind = _telemetrySettingsDto.XAxisKind;
            graphViewModel.GraphViewSynchronization = _graphViewSynchronization;
            graphViewModel.LapColorSynchronization = _lapColorSynchronization;
            graphViewModel.SuspensionDistanceUnits = _settingsProvider.DisplaySettingsViewModel.DistanceUnitsVerySmall;
            graphViewModel.DistanceUnits = _settingsProvider.DisplaySettingsViewModel.DistanceUnitsSmall;
            graphViewModel.VelocityUnits = _settingsProvider.DisplaySettingsViewModel.VelocityUnits;
            graphViewModel.VelocityUnitsSmall = _settingsProvider.DisplaySettingsViewModel.VelocityUnitsVerySmall;
            graphViewModel.TemperatureUnits = _settingsProvider.DisplaySettingsViewModel.TemperatureUnits;
            graphViewModel.PressureUnits = _settingsProvider.DisplaySettingsViewModel.PressureUnits;
            graphViewModel.AngleUnits = _settingsProvider.DisplaySettingsViewModel.AngleUnits;
            graphViewModel.ForceUnits = _settingsProvider.DisplaySettingsViewModel.ForceUnits;
            _loadedLaps.ForEach(graphViewModel.AddLapTelemetry);
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.SyncTelemetryView += TelemetryViewsSynchronizationOnSyncTelemetryView;
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetrySettingsRepository.SettingsChanged += TelemetrySettingsRepositoryOnSettingsChanged;
        }

        private void Unsubscribe()
        {
            _telemetryViewsSynchronization.SyncTelemetryView -= TelemetryViewsSynchronizationOnSyncTelemetryView;
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetrySettingsRepository.SettingsChanged -= TelemetrySettingsRepositoryOnSettingsChanged;
        }

        private void TelemetrySettingsRepositoryOnSettingsChanged(object sender, EventArgs e)
        {
            _telemetrySettingsDto = _telemetrySettingsRepository.LoadOrCreateNew();
            ReloadGraphCollection();
            Graphs.ForEach(InitializeViewModel);
        }

        private void TelemetryViewsSynchronizationOnSyncTelemetryView(object sender, TelemetrySnapshotArgs e)
        {
            if (e.LapSummaryDto == null)
            {
                return;
            }

            Graphs.ForEach(x => x.UpdateXSelection(e.LapSummaryDto.Id, e.TelemetrySnapshot));
        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            _loadedLaps.RemoveAll(x => x.LapSummary.Id == e.LapSummary.Id);
            Graphs.ForEach(x => x.RemoveLapTelemetry(e.LapSummary));
        }
        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            _loadedLaps.Add(e.LapTelemetry);
            Graphs.ForEach(x => x.AddLapTelemetry(e.LapTelemetry));
        }

        protected abstract void ReloadGraphCollection();
    }
}