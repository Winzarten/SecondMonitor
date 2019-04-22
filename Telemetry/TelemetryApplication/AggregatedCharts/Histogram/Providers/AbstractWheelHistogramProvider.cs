namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.Commands;
    using Extractors;
    using SecondMonitor.ViewModels.Factory;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.Histogram;
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

        protected virtual IAggregatedChartViewModel CreateAggregatedChartViewModel<T, TX>() where T : WheelsHistogramChartViewModel, new() where TX : HistogramChartViewModel, new()
        {
            List<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps.ToList();
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            T wheelsHistogram = _viewModelFactory.Create<T>();

            wheelsHistogram.Title = title;
            wheelsHistogram.BandSize = _abstractWheelHistogramDataExtractor.DefaultBandSize;
            wheelsHistogram.Unit = _abstractWheelHistogramDataExtractor.YUnit;

            wheelsHistogram.RefreshCommand = new RelayCommand(() => FillHistogramViewmodel<TX>(loadedLaps, wheelsHistogram.BandSize, wheelsHistogram));

            FillHistogramViewmodel<TX>(loadedLaps, _abstractWheelHistogramDataExtractor.DefaultBandSize, wheelsHistogram);

            return wheelsHistogram;
        }

        public virtual IAggregatedChartViewModel CreateAggregatedChartViewModel() => CreateAggregatedChartViewModel<WheelsHistogramChartViewModel, HistogramChartViewModel>();


        protected void FillHistogramViewmodel<T>(IReadOnlyCollection<LapTelemetryDto> loadedLaps, double bandSize, WheelsChartViewModel wheelsChart) where T : HistogramChartViewModel, new()
        {
            Histogram flHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramFrontLeft(loadedLaps, bandSize);
            Histogram frHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramFrontRight(loadedLaps, bandSize);
            Histogram rlHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramRearLeft(loadedLaps, bandSize);
            Histogram rrHistogram = _abstractWheelHistogramDataExtractor.ExtractHistogramRearRight(loadedLaps, bandSize);

            T flViewModel = new T();
            flViewModel.FromModel(flHistogram);

            T frViewModel = new T();
            frViewModel.FromModel(frHistogram);

            T rlViewModel = new T();
            rlViewModel.FromModel(rlHistogram);

            T rrViewModel = new T();
            rrViewModel.FromModel(rrHistogram);

            wheelsChart.FrontLeftChartViewModel = flViewModel;
            wheelsChart.FrontRightChartViewModel = frViewModel;
            wheelsChart.RearLeftChartViewModel = rlViewModel;
            wheelsChart.RearRightChartViewModel = rrViewModel;
        }
    }
}