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

        protected AbstractGraphPanelController(IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
        }

        public abstract bool IsLetPanel { get; }

        protected abstract IGraphViewModel[] Graphs { get; }

        public void StartController()
        {
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
        }

        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            Graphs.ForEach(x => x.FromModel(e.LapTelemetry));
        }

        private void Unsubscribe()
        {
            _telemetryViewsSynchronization.LapLoaded -= TelemetryViewsSynchronizationOnLapLoaded;
        }

        protected void RefreshViewModels()
        {
            _mainWindowViewModel.ClearLeftPanelGraphs();;
            _mainWindowViewModel.AddToLeftPanelGraphs(Graphs);
        }
    }
}