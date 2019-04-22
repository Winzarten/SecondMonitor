namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Extractors;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public class RpmToHorizontalGChartProvider : IAggregatedChartProvider
    {
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly RpmToHorizontalGExtractor _rpmToHorizontalGExtractor;
        public string ChartName => "Horizontal Acceleration (RPM)";
        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public RpmToHorizontalGChartProvider(ILoadedLapsCache loadedLapsCache, RpmToHorizontalGExtractor rpmToHorizontalGExtractor)
        {
            _loadedLapsCache = loadedLapsCache;
            _rpmToHorizontalGExtractor = rpmToHorizontalGExtractor;
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            int maxGear = loadedLaps.SelectMany(x => x.TimedTelemetrySnapshots).Where(x => !string.IsNullOrWhiteSpace(x.PlayerData.CarInfo.CurrentGear) && x.PlayerData.CarInfo.CurrentGear != "R" && x.PlayerData.CarInfo.CurrentGear != "N").Max(x => int.Parse(x.PlayerData.CarInfo.CurrentGear));

            CompositeAggregatedChartsViewModel viewModel = new CompositeAggregatedChartsViewModel() { Title = title };

            ScatterPlotChartViewModel mainViewModel = new ScatterPlotChartViewModel() { Title = "All Gear" };
            mainViewModel.FromModel(CreateScatterPlotAllGear(loadedLaps, maxGear));

            viewModel.MainAggregatedChartViewModel = mainViewModel;

            for (int i = 1; i <= maxGear; i++)
            {
                ScatterPlot scatterPlot = CreateScatterPlot(loadedLaps, i);
                if (scatterPlot.ScatterPlotSeries.Count == 0)
                {
                    continue;
                }

                ScatterPlotChartViewModel child = new ScatterPlotChartViewModel() { Title = $"Gear {i}" };
                child.FromModel(scatterPlot);
                viewModel.AddChildAggregatedChildViewModel(child);
            }

            return viewModel;
        }

        protected ScatterPlot CreateScatterPlot(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int gear)
        {
            AxisDefinition xAxis = new AxisDefinition(_rpmToHorizontalGExtractor.XMajorTickSize, _rpmToHorizontalGExtractor.XMajorTickSize / 4, _rpmToHorizontalGExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_rpmToHorizontalGExtractor.YMajorTickSize, _rpmToHorizontalGExtractor.YMajorTickSize / 4, _rpmToHorizontalGExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot($"Gear: {gear}", xAxis, yAxis);

            scatterPlot.AddScatterPlotSeries(_rpmToHorizontalGExtractor.ExtractSeriesForGear(loadedLaps, gear.ToString()));
            return scatterPlot;
        }

        protected ScatterPlot CreateScatterPlotAllGear(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int maxGear)
        {
            AxisDefinition xAxis = new AxisDefinition(_rpmToHorizontalGExtractor.XMajorTickSize, _rpmToHorizontalGExtractor.XMajorTickSize / 4, _rpmToHorizontalGExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_rpmToHorizontalGExtractor.YMajorTickSize, _rpmToHorizontalGExtractor.YMajorTickSize / 4, _rpmToHorizontalGExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot("All Gears", xAxis, yAxis);

            for (int i = 1; i <= maxGear; i++)
            {
                scatterPlot.AddScatterPlotSeries(_rpmToHorizontalGExtractor.ExtractSeriesForGear(loadedLaps, i.ToString()));
            }

            return scatterPlot;

        }
    }
}