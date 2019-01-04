namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Linq;
    using Settings;
    using Synchronization;
    using Synchronization.Graphs;
    using ViewModels;
    using ViewModels.GraphPanel;

    public class RightGraphPanelController : AbstractGraphPanelController
    {
        private readonly IGraphViewModelsProvider _graphViewModelsProvider;

        public RightGraphPanelController(IGraphViewModelsProvider graphViewModelsProvider, IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization, ISettingsProvider settingsProvider, IGraphViewSynchronization graphViewSynchronization)
            : base(mainWindowViewModel, telemetryViewsSynchronization, lapColorSynchronization, settingsProvider, graphViewSynchronization)
        {
            _graphViewModelsProvider = graphViewModelsProvider;
            Graphs = _graphViewModelsProvider.GetRightSideViewModels().ToArray();
        }

        public override bool IsLetPanel => false;

        protected override IGraphViewModel[] Graphs { get; }

        protected override void RefreshViewModels()
        {
            MainWindowViewModel.ClearRightPanelGraphs();
            MainWindowViewModel.AddToRightPanelGraphs(Graphs);
        }
    }
}