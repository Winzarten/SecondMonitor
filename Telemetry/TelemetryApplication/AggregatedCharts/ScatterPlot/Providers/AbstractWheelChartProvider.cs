namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Extensions;
    using Extractors;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.ScatterPlot;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractWheelChartProvider : IAggregatedChartProvider
    {
        private readonly AbstractWheelScatterPlotDataExtractor _dataExtractor;
        private readonly ILoadedLapsCache _loadedLaps;
        public abstract string ChartName { get; }

        public AggregatedChartKind Kind => AggregatedChartKind.ScatterPlot;

        protected AbstractWheelChartProvider(AbstractWheelScatterPlotDataExtractor dataExtractor, ILoadedLapsCache loadedLaps)
        {
            _dataExtractor = dataExtractor;
            _loadedLaps = loadedLaps;
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLaps.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";
            ScatterPlotSeries frontLeftSeries = _dataExtractor.ExtractFrontLeft(loadedLaps);
            ScatterPlotSeries frontRightSeries = _dataExtractor.ExtractFrontRight(loadedLaps);
            ScatterPlotSeries rearLeftSeries = _dataExtractor.ExtractRearLeft(loadedLaps);
            ScatterPlotSeries rearRightSeries = _dataExtractor.ExtractRearRight(loadedLaps);

            SplitAggregatedChartViewModel mainViewModel = new SplitAggregatedChartViewModel()
            {
                Title = title,
                TopViewModel = CreateScatterPlotChartViewModel("All Wheels", frontLeftSeries, frontRightSeries, rearLeftSeries, rearRightSeries),
                BottomViewModel = CreateWheelsChartViewModel(frontLeftSeries, frontRightSeries, rearLeftSeries, rearRightSeries)
            };
            return mainViewModel;
        }

        private WheelsChartViewModel CreateWheelsChartViewModel(ScatterPlotSeries fl, ScatterPlotSeries fr, ScatterPlotSeries rl, ScatterPlotSeries rr)
        {
            WheelsChartViewModel wheelsChartViewModel = new WheelsChartViewModel
            {
                FrontLeftChartViewModel = CreateScatterPlotChartViewModel("Front Left", fl),
                FrontRightChartViewModel = CreateScatterPlotChartViewModel("Front Right", fr),
                RearLeftChartViewModel = CreateScatterPlotChartViewModel("Rear Left", rl),
                RearRightChartViewModel = CreateScatterPlotChartViewModel("Rear Right", rr)
            };
            return wheelsChartViewModel;
        }

        private ScatterPlotChartViewModel CreateScatterPlotChartViewModel(string title, params ScatterPlotSeries[] series)
        {

            ScatterPlot scatterPlot = new ScatterPlot(title, CreateXAxisDefinition(), CreateYAxisDefinition());
            series.ForEach(scatterPlot.AddScatterPlotSeries);

            ScatterPlotChartViewModel viewModel = new ScatterPlotChartViewModel() {Title = title};
            viewModel.FromModel(scatterPlot);
            return viewModel;

        }

        private AxisDefinition CreateXAxisDefinition()
        {
            return new AxisDefinition(_dataExtractor.XMajorTickSize, _dataExtractor.XMajorTickSize / 4, _dataExtractor.XUnit);
        }

        private AxisDefinition CreateYAxisDefinition()
        {
            return new AxisDefinition(_dataExtractor.YMajorTickSize, _dataExtractor.YMajorTickSize / 4, _dataExtractor.YUnit);
        }
    }
}