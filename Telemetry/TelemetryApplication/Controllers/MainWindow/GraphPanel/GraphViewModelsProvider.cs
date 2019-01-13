namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using Factory;
    using Settings;
    using ViewModels.GraphPanel;

    public class GraphViewModelsProvider : IGraphViewModelsProvider
    {
        private readonly IGraphsSettingsProvider _graphsSettingsProvider;

        private readonly IViewModelFactory _viewModelFactory;

        public GraphViewModelsProvider(IViewModelFactory viewModelFactory, IGraphsSettingsProvider graphsSettingsProvider)
        {
            _viewModelFactory = viewModelFactory;
            _graphsSettingsProvider = graphsSettingsProvider;
        }

        public IEnumerable<(IGraphViewModel graphViewModel, int priority)> GetLeftSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>().Where(_graphsSettingsProvider.IsGraphViewModelLeft).Select(x => (x, _graphsSettingsProvider.GetGraphPriority(x)));
        }

        public IEnumerable<(IGraphViewModel graphViewModel, int priority)> GetRightSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>().Where(_graphsSettingsProvider.IsGraphViewModelRight).Select(x => (x, _graphsSettingsProvider.GetGraphPriority(x)));
        }
    }
}