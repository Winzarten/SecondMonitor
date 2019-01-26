namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Linq;
    using DataModel.Extensions;
    using Settings;
    using Synchronization;
    using Synchronization.Graphs;
    using ViewModels;
    using ViewModels.GraphPanel;

    public class LeftGraphPanelController : AbstractGraphPanelController
    {
        private readonly IGraphViewModelsProvider _graphViewModelsProvider;

        public LeftGraphPanelController(IGraphViewModelsProvider graphViewModelsProvider, IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization, ILapColorSynchronization lapColorSynchronization,
            ISettingsProvider settingsProvider, IGraphViewSynchronization graphViewSynchronization, ITelemetrySettingsRepository telemetrySettingsRepository)
            : base(mainWindowViewModel, telemetryViewsSynchronization, lapColorSynchronization, settingsProvider, graphViewSynchronization, telemetrySettingsRepository)
        {
            _graphViewModelsProvider = graphViewModelsProvider;
        }

        public override bool IsLetPanel => true;

        protected override IGraphViewModel[] Graphs { get; set; }

        protected override void ReloadGraphCollection()
        {
            IGraphViewModel[] newGraphs = _graphViewModelsProvider.GetLeftSideViewModels().OrderBy(x => x.priority).Select(y => y.graphViewModel).ToArray();
            MainWindowViewModel.ClearLeftPanelGraphs();
            Graphs?.ForEach(x => x.Dispose());
            Graphs = newGraphs;
            MainWindowViewModel.AddToLeftPanelGraphs(Graphs);
        }
    }
}