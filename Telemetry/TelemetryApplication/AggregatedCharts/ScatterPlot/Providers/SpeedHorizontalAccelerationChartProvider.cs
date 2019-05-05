namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Extractors;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public class SpeedHorizontalAccelerationChartProvider : IAggregatedChartProvider
    {
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly SpeedToHorizontalGExtractor _speedToHorizontalGExtractor;
        public string ChartName => "Longitudinal Acceleration (Speed)";
        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public SpeedHorizontalAccelerationChartProvider(ILoadedLapsCache loadedLapsCache, SpeedToHorizontalGExtractor speedToHorizontalGExtractor)
        {
            _loadedLapsCache = loadedLapsCache;
            _speedToHorizontalGExtractor = speedToHorizontalGExtractor;
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
            AxisDefinition xAxis = new AxisDefinition(_speedToHorizontalGExtractor.XMajorTickSize, _speedToHorizontalGExtractor.XMajorTickSize / 5, _speedToHorizontalGExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_speedToHorizontalGExtractor.YMajorTickSize, _speedToHorizontalGExtractor.YMajorTickSize / 5, _speedToHorizontalGExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot($"Gear: {gear}", xAxis, yAxis);

            scatterPlot.AddScatterPlotSeries(_speedToHorizontalGExtractor.ExtractSeriesForGear(loadedLaps, gear.ToString()));
            return scatterPlot;
        }

        protected ScatterPlot CreateScatterPlotAllGear(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int maxGear)
        {
            AxisDefinition xAxis = new AxisDefinition(_speedToHorizontalGExtractor.XMajorTickSize, _speedToHorizontalGExtractor.XMajorTickSize / 5, _speedToHorizontalGExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_speedToHorizontalGExtractor.YMajorTickSize, _speedToHorizontalGExtractor.YMajorTickSize / 5, _speedToHorizontalGExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot("All Gears", xAxis, yAxis);

            for (int i = 1; i <= maxGear; i++)
            {
                scatterPlot.AddScatterPlotSeries(_speedToHorizontalGExtractor.ExtractSeriesForGear(loadedLaps, i.ToString()));
            }

            return scatterPlot;

        }
    }
}