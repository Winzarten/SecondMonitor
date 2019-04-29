namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extractors;
    using Filter;
    using OxyPlot;
    using OxyPlot.Annotations;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public class RearRollAngleToFrontRollAngleProvider : IAggregatedChartProvider
    {
        protected static readonly List<OxyColor> ColorMap = new List<OxyColor>()
        {
            OxyColor.Parse("#ff9900"),
            OxyColor.Parse("#99ffcc"),
            OxyColor.Parse("#0066ff"),
            OxyColor.Parse("#33cccc"),
            OxyColor.Parse("#E5FBE4"),
            OxyColor.Parse("#ff531a"),
            OxyColor.Parse("#1aff1a"),
            OxyColor.Parse("#0000ff"),
            OxyColor.Parse("#cccc00"),
        };

        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly RearRollAngleToFrontRollAngleExtractor _dataExtractor;
        private readonly LateralAccFilter _lateralAccFilter;
        private readonly List<ITelemetryFilter> _filters;

        public string ChartName => "Rear Roll Angle / Front Roll Angle ";

        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public RearRollAngleToFrontRollAngleProvider(ILoadedLapsCache loadedLapsCache, RearRollAngleToFrontRollAngleExtractor dataExtractor, LateralAccFilter lateralAccFilter)
        {
            _loadedLapsCache = loadedLapsCache;
            _dataExtractor = dataExtractor;
            _lateralAccFilter = lateralAccFilter;
            _filters = new List<ITelemetryFilter>() {_lateralAccFilter};
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            AxisDefinition xAxis = new AxisDefinition(_dataExtractor.XMajorTickSize, _dataExtractor.XMajorTickSize / 4, _dataExtractor.XUnit, "Rear Roll Angle");
            AxisDefinition yAxis = new AxisDefinition(_dataExtractor.YMajorTickSize, _dataExtractor.YMajorTickSize / 4, _dataExtractor.YUnit, "Front Roll Angle");
            ScatterPlot scatterPlot = new ScatterPlot(title, xAxis, yAxis);

            for (int i = 0; i < ColorMap.Count; i++)
            {
                double minG = i * 0.25;
                double maxG = i + 1 == ColorMap.Count ? double.MaxValue : (i + 1) * 0.25;
                _lateralAccFilter.MinimumG = minG;
                _lateralAccFilter.MaximumG = maxG;
                string seriesTitle = maxG < double.MaxValue ? $"{minG:F2}G - {maxG:F2}G" : $"{minG:F2}G+";
                ScatterPlotSeries newSeries = _dataExtractor.ExtractSeries(loadedLaps, _filters, seriesTitle, ColorMap[i]);
                if (newSeries == null)
                {
                    continue;
                }

                scatterPlot.AddScatterPlotSeries(newSeries);
            }
            scatterPlot.AddAnnotation(new LineAnnotation() { Slope = 1, Intercept = 0, Color = OxyColors.Red, StrokeThickness = 1, LineStyle = LineStyle.Solid });
            ScatterPlotChartViewModel viewModel = new ScatterPlotChartViewModel() {Title = title};
            viewModel.FromModel(scatterPlot);

            return viewModel;
        }
    }
}