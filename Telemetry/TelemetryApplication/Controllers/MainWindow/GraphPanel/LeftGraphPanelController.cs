namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Linq;
    using Settings;
    using Synchronization;
    using Synchronization.Graphs;
    using ViewModels;
    using ViewModels.GraphPanel;

    public class LeftGraphPanelController : AbstractGraphPanelController
    {
        private readonly IGraphViewModelsProvider _graphViewModelsProvider;

        public LeftGraphPanelController(IGraphViewModelsProvider graphViewModelsProvider, IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization, ISettingsProvider settingsProvider, IGraphViewSynchronization graphViewSynchronization)
            : base(mainWindowViewModel, telemetryViewsSynchronization, lapColorSynchronization, settingsProvider, graphViewSynchronization)
        {
            _graphViewModelsProvider = graphViewModelsProvider;
            Graphs = _graphViewModelsProvider.GetLeftSideViewModels().OrderBy(x => x.priority).Select(y => y.graphViewModel).ToArray();
        }

        public override bool IsLetPanel => true;

        protected override IGraphViewModel[] Graphs { get; }

        protected override void RefreshViewModels()
        {
            MainWindowViewModel.ClearLeftPanelGraphs();
            MainWindowViewModel.AddToLeftPanelGraphs(Graphs);
        }
    }
}