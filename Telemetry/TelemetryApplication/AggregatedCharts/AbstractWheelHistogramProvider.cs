namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.Commands;
    using Histogram;
    using SecondMonitor.ViewModels.Factory;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.Histogram;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractWheelHistogramProvider : IAggregatedChartProvider
    {
        private readonly AbstractWheelHistogramDataExtractor _abstractWheelHistogramDataExtractor;
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly IViewModelFactory _viewModelFactory;

        protected AbstractWheelHistogramProvider(AbstractWheelHistogramDataExtractor abstractWheelHistogramDataExtractor, ILoadedLapsCache loadedLapsCache, IViewModelFactory viewModelFactory)
        {
            _abstractWheelHistogramDataExtractor = abstractWheelHistogramDataExtractor;
            _loadedLapsCache = loadedLapsCache;
            _viewModelFactory = viewModelFactory;
        }

        public abstract string ChartName { get; }
        public abstract AggregatedChartKind Kind { get; }

        protected virtual IAggregatedChartViewModel CreateAggregatedChartViewModel<T, TX>() where T : AbstractWheelsHistogramViewModel<TX>, new() where TX : HistogramChartViewModel, new()
        {
            List<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps.ToList();
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            T wheelsHistogram = _viewModelFactory.Create<T>();

            wheelsHistogram.Title = title;
            wheelsHistogram.BandSize = _abstractWheelHistogramDataExtractor.DefaultBandSize;
            wheelsHistogram.Unit = _abstractWheelHistogramDataExtractor.YUnit;

            wheelsHistogram.RefreshCommand = new RelayCommand(() => FillHistogramViewmodel(loadedLaps, wheelsHistogram.BandSize, wheelsHistogram));

            FillHistogramViewmodel(loadedLaps, _abstractWheelHistogramDataExtractor.DefaultBandSize, wheelsHistogram);

            return wheelsHistogram;
        }

        public virtual IAggregatedChartViewModel CreateAggregatedChartViewModel() => CreateAggregatedChartViewModel<WheelsHistogramViewModel, HistogramChartViewModel>();


        protected void FillHistogramViewmodel<TX>(IReadOnlyCollection<LapTelemetryDto> loadedLaps, double bandSize, AbstractWheelsHistogramViewModel<TX> wheelsHistogram) where TX : HistogramChartViewModel, new()
        {
            Histogram.Histogram flHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramFrontLeft(loadedLaps, bandSize);
            Histogram.Histogram frHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramFrontRight(loadedLaps, bandSize);
            Histogram.Histogram rlHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramRearLeft(loadedLaps, bandSize);
            Histogram.Histogram rrHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramRearRight(loadedLaps, bandSize);
            wheelsHistogram.FrontLeftChartViewModel.FromModel(flHistogram);
            wheelsHistogram.FrontRightChartViewModel.FromModel(frHistogram);
            wheelsHistogram.RearLeftChartViewModel.FromModel(rlHistogram);
            wheelsHistogram.RearRightChartViewModel.FromModel(rrHistogram);
        }
    }
}