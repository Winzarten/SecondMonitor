namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.AggregatedChart
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using AggregatedCharts;
    using Contracts.Commands;
    using ViewModels;
    using ViewModels.GraphPanel;
    using ViewModels.LoadedLapCache;

    public class AggregatedChartsController : IAggregatedChartsController
    {

        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly List<IAggregatedChartProvider> _aggregatedChartProviders;

        public AggregatedChartsController(IEnumerable<IAggregatedChartProvider> aggregatedChartProviders, IMainWindowViewModel mainWindowViewModel, ILoadedLapsCache loadedLapsCache)
        {
            _loadedLapsCache = loadedLapsCache;
            _mainWindowViewModel = mainWindowViewModel;
            _loadedLapsCache = loadedLapsCache;
            _aggregatedChartProviders = aggregatedChartProviders.ToList();
        }

        public Task StartControllerAsync()
        {
            BindCommands();
            return Task.CompletedTask;
        }

        private void BindCommands()
        {
            _mainWindowViewModel.LapSelectionViewModel.OpenAggregatedChartSelectorCommand = new RelayCommand(OpenAggregatedChartSelector);
        }

        private void OpenAggregatedChartSelector()
        {
            if (_loadedLapsCache.LoadedLaps.Count == 0)
            {
                return;
            }
            AggregatedChartViewModel viewModel = _aggregatedChartProviders.First().CreateAggregatedChartViewModel();
            var win = new Window { Content = viewModel, Title = viewModel.Title, WindowState = WindowState.Maximized };
            win.Show();
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }


    }
}