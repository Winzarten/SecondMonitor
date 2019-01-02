namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using DataModel.Extensions;
    using Synchronization;
    using ViewModels;
    using ViewModels.GraphPanel;

    public abstract class AbstractGraphPanelController : IGraphPanelController
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly ILapColorSynchronization _lapColorSynchronization;

        protected AbstractGraphPanelController(IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _lapColorSynchronization = lapColorSynchronization;
        }

        public abstract bool IsLetPanel { get; }

        protected abstract IGraphViewModel[] Graphs { get; }

        public void StartController()
        {
            Graphs.ForEach(x => x.LapColorSynchronization = _lapColorSynchronization);
            Subscribe();
            RefreshViewModels();
        }

        public void StopController()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
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
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded -= TelemetryViewsSynchronizationOnLapUnloaded;
        }

        protected void RefreshViewModels()
        {
            _mainWindowViewModel.ClearLeftPanelGraphs();;
            _mainWindowViewModel.AddToLeftPanelGraphs(Graphs);
        }
    }
}