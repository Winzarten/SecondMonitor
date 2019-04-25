namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Extractors;
    using Filter;
    using OxyPlot;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public class SpeedToRakeProvider : IAggregatedChartProvider
    {
        private readonly SpeedToRakeExtractor _dataExtractor;
        private readonly ILoadedLapsCache _loadedLapsCache;

        public SpeedToRakeProvider(SpeedToRakeExtractor dataExtractor, ILoadedLapsCache loadedLapsCache)
        {
            _dataExtractor = dataExtractor;
            _loadedLapsCache = loadedLapsCache;
        }

        public string ChartName => "Rake / Speed";
        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            AxisDefinition xAxis = new AxisDefinition(_dataExtractor.XMajorTickSize, _dataExtractor.XMajorTickSize / 4, _dataExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_dataExtractor.YMajorTickSize, _dataExtractor.YMajorTickSize / 4, _dataExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot(title, xAxis, yAxis);

            scatterPlot.AddScatterPlotSeries(_dataExtractor.ExtractSeries(loadedLaps, Enumerable.Empty<ITelemetryFilter>().ToList(), title, OxyColors.Green));

            ScatterPlotChartViewModel viewModel = new ScatterPlotChartViewModel() {Title = "Rake"};
            viewModel.FromModel(scatterPlot);

            return viewModel;
        }
    }
}