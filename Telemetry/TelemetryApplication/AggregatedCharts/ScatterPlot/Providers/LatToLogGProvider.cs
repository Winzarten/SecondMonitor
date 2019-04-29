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

    public class LatToLogGProvider : IAggregatedChartProvider
    {
        protected static readonly List<OxyColor> ColorMap = new List<OxyColor>()
        {
            OxyColor.Parse("#ff9900"),
            OxyColor.Parse("#99ffcc"),
            OxyColors.Red,
            OxyColor.Parse("#0000ff"),
            OxyColor.Parse("#E5FBE4"),
        };

        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly LateralToLongGExtractor _dataExtractor;
        private readonly ThrottlePositionFilter _throttlePositionFilter;
        private readonly List<ITelemetryFilter> _filters;

        public string ChartName => "Lateral / Longitudinal G ";

        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public LatToLogGProvider(ILoadedLapsCache loadedLapsCache, LateralToLongGExtractor dataExtractor, ThrottlePositionFilter throttlePositionFilter)
        {
            _loadedLapsCache = loadedLapsCache;
            _dataExtractor = dataExtractor;
            _throttlePositionFilter = throttlePositionFilter;
            _filters = new List<ITelemetryFilter>() { throttlePositionFilter };
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            AxisDefinition xAxis = new AxisDefinition(_dataExtractor.XMajorTickSize, _dataExtractor.XMajorTickSize / 4, _dataExtractor.XUnit, "Lat Acc");
            AxisDefinition yAxis = new AxisDefinition(_dataExtractor.YMajorTickSize, _dataExtractor.YMajorTickSize / 4, _dataExtractor.YUnit, "Long Acc");
            ScatterPlot scatterPlot = new ScatterPlot(title, xAxis, yAxis);

            for (int i = 0; i < 4; i++)
            {
                _throttlePositionFilter.Minimum = i * 0.25; ;
                _throttlePositionFilter.Maximum = (i + 1) * 0.25;
                string seriesTitle = $"Throttle - {i * 25}% - {(i + 1) * 25:F2}%";
                scatterPlot.AddScatterPlotSeries(_dataExtractor.ExtractSeries(loadedLaps, _filters, seriesTitle, ColorMap[i]));
            }

            _throttlePositionFilter.Minimum = 1;
            _throttlePositionFilter.Maximum = double.MaxValue;
            scatterPlot.AddScatterPlotSeries(_dataExtractor.ExtractSeries(loadedLaps, _filters, "Throttle - 100%", ColorMap[4]));

            ScatterPlotChartViewModel viewModel = new ScatterPlotChartViewModel() { Title = "Lateral / Longitudinal G" };
            viewModel.FromModel(scatterPlot);

            return viewModel;
        }
    }
}