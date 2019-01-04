namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using Factory;
    using Settings;
    using ViewModels.GraphPanel;

    public class GraphViewModelsProvider : IGraphViewModelsProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        private readonly IViewModelFactory _viewModelFactory;

        public GraphViewModelsProvider(IViewModelFactory viewModelFactory, ISettingsProvider settingsProvider)
        {
            _viewModelFactory = viewModelFactory;
            _settingsProvider = settingsProvider;
        }

        public IEnumerable<IGraphViewModel> GetLeftSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>().Where(_settingsProvider.GraphsSettingsProvider.IsGraphViewModelLeft);
        }

        public IEnumerable<IGraphViewModel> GetRightSideViewModels()
        {
            return _viewModelFactory.CreateAll<IGraphViewModel>().Where(_settingsProvider.GraphsSettingsProvider.IsGraphViewModelRight);
        }
    }
}