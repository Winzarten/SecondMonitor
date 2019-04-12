namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.Commands;
    using Histogram;
    using TelemetryManagement.DTO;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.Histogram;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractWheelHistogramProvider : IAggregatedChartProvider
    {
        private readonly AbstractWheelHistogramDataExtractor _abstractWheelHistogramDataExtractor;
        private readonly ILoadedLapsCache _loadedLapsCache;

        protected AbstractWheelHistogramProvider(AbstractWheelHistogramDataExtractor abstractWheelHistogramDataExtractor, ILoadedLapsCache loadedLapsCache)
        {
            _abstractWheelHistogramDataExtractor = abstractWheelHistogramDataExtractor;
            _loadedLapsCache = loadedLapsCache;
        }

        public abstract string ChartName { get; }

        public AggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            List<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps.ToList();
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            WheelsHistogramViewModel wheelsHistogram = new WheelsHistogramViewModel()
            {
                Title = title,
                BandSize = _abstractWheelHistogramDataExtractor.DefaultBandSize,
                Unit = _abstractWheelHistogramDataExtractor.Unit,

            };

            wheelsHistogram.RefreshCommand = new RelayCommand(() => FillHistogramViewmodel(loadedLaps, wheelsHistogram.BandSize, wheelsHistogram));

            FillHistogramViewmodel(loadedLaps, _abstractWheelHistogramDataExtractor.DefaultBandSize, wheelsHistogram);

            return wheelsHistogram;
        }

        protected void FillHistogramViewmodel(IReadOnlyCollection<LapTelemetryDto> loadedLaps, double bandSize, WheelsHistogramViewModel wheelsHistogram)
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