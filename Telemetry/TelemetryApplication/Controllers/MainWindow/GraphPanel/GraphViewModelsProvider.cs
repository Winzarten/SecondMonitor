namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Extensions;
    using SecondMonitor.ViewModels.Factory;
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
            List<IGraphViewModel> allCharts =  _viewModelFactory.CreateAll<IGraphViewModel>().ToList();
            List<IGraphViewModel> chartsToReturn = allCharts.Where(_graphsSettingsProvider.IsGraphViewModelLeft).ToList();

            allCharts.Except(chartsToReturn).ForEach(x => x.Dispose());

            return chartsToReturn.Select(x => (x, _graphsSettingsProvider.GetGraphPriority(x)));
        }

        public IEnumerable<(IGraphViewModel graphViewModel, int priority)> GetRightSideViewModels()
        {
            List<IGraphViewModel> allCharts = _viewModelFactory.CreateAll<IGraphViewModel>().ToList();
            List<IGraphViewModel> chartsToReturn = allCharts.Where(_graphsSettingsProvider.IsGraphViewModelRight).ToList();

            allCharts.Except(chartsToReturn).ForEach(x => x.Dispose());

            return chartsToReturn.Select(x => (x, _graphsSettingsProvider.GetGraphPriority(x)));
        }
    }
}