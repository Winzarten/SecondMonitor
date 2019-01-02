namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using Factory;
    using ViewModels.GraphPanel;

    public class GraphViewModelsProvider : IGraphViewModelsProvider
    {
        private readonly IViewModelFactory _viewModelFactory;

        public GraphViewModelsProvider(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public IEnumerable<IGraphViewModel> GetLeftSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>();
        }

        public IEnumerable<IGraphViewModel> GetRightSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>();
        }
    }
}