namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Linq;
    using Synchronization;
    using ViewModels;
    using ViewModels.GraphPanel;

    public class LeftGraphPanelController : AbstractGraphPanelController
    {
        private readonly IGraphViewModelsProvider _graphViewModelsProvider;

        public LeftGraphPanelController(IGraphViewModelsProvider graphViewModelsProvider, IMainWindowViewModel mainWindowViewModel, ITelemetryViewsSynchronization telemetryViewsSynchronization) : base(mainWindowViewModel, telemetryViewsSynchronization)
        {
            _graphViewModelsProvider = graphViewModelsProvider;
            Graphs = _graphViewModelsProvider.GetLeftSideViewModels().ToArray();
        }

        public override bool IsLetPanel => true;

        protected override IGraphViewModel[] Graphs { get; }
    }
}