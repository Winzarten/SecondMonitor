namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public class SpeedToRpmChartProvider : IAggregatedChartProvider
    {
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly SpeedToRpmScatterPlotExtractor _speedToRpmScatterPlotExtractor;
        public string ChartName => "Speed vs RPM Scatter Plot";
        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        public SpeedToRpmChartProvider(ILoadedLapsCache loadedLapsCache, SpeedToRpmScatterPlotExtractor speedToRpmScatterPlotExtractor)
        {
            _loadedLapsCache = loadedLapsCache;
            _speedToRpmScatterPlotExtractor = speedToRpmScatterPlotExtractor;
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            int maxGear = loadedLaps.SelectMany(x => x.TimedTelemetrySnapshots).Where(x => !string.IsNullOrWhiteSpace(x.PlayerData.CarInfo.CurrentGear) && x.PlayerData.CarInfo.CurrentGear != "R" && x.PlayerData.CarInfo.CurrentGear != "N").Max(x => int.Parse(x.PlayerData.CarInfo.CurrentGear));

            CompositeAggregatedChartsViewModel viewModel = new CompositeAggregatedChartsViewModel() {Title = title};

            ScatterPlotChartViewModel mainViewModel = new ScatterPlotChartViewModel(){Title = "All Gear"};
            mainViewModel.FromModel(CreateScatterPlotAllGear(loadedLaps,maxGear));

            viewModel.MainAggregatedChartViewModel = mainViewModel;

            for (int i = 1; i <= maxGear; i++)
            {
                ScatterPlot scatterPlot = CreateScatterPlot(loadedLaps, i);
                if (scatterPlot.ScatterPlotSeries.Count == 0)
                {
                    continue;
                }

                ScatterPlotChartViewModel child = new ScatterPlotChartViewModel() {Title = $"Gear {i}"};
                child.FromModel(scatterPlot);
                viewModel.AddChildAggregatedChildViewModel(child);
            }

            return viewModel;
        }

        protected ScatterPlot CreateScatterPlot(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int gear)
        {
            AxisDefinition xAxis = new AxisDefinition(_speedToRpmScatterPlotExtractor.XMajorTickSize, _speedToRpmScatterPlotExtractor.XMajorTickSize / 5, _speedToRpmScatterPlotExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_speedToRpmScatterPlotExtractor.YMajorTickSize, _speedToRpmScatterPlotExtractor.YMajorTickSize / 5, _speedToRpmScatterPlotExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot($"Gear: {gear}", xAxis, yAxis);

            scatterPlot.AddScatterPlotSeries(_speedToRpmScatterPlotExtractor.ExtractSeriesForGear(loadedLaps, gear.ToString()));
            return scatterPlot;
        }

        protected ScatterPlot CreateScatterPlotAllGear(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int maxGear)
        {
            AxisDefinition xAxis = new AxisDefinition(_speedToRpmScatterPlotExtractor.XMajorTickSize, _speedToRpmScatterPlotExtractor.XMajorTickSize / 5, _speedToRpmScatterPlotExtractor.XUnit);
            AxisDefinition yAxis = new AxisDefinition(_speedToRpmScatterPlotExtractor.YMajorTickSize, _speedToRpmScatterPlotExtractor.YMajorTickSize / 5, _speedToRpmScatterPlotExtractor.YUnit);
            ScatterPlot scatterPlot = new ScatterPlot("All Gears", xAxis, yAxis);

            for (int i = 1; i <= maxGear; i++)
            {
                scatterPlot.AddScatterPlotSeries(_speedToRpmScatterPlotExtractor.ExtractSeriesForGear(loadedLaps, i.ToString()));
            }

            return scatterPlot;

        }
    }
}