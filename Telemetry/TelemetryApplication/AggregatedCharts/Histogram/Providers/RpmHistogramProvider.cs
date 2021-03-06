﻿namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Extractors;
    using SecondMonitor.ViewModels.Factory;
    using TelemetryManagement.DTO;
    using ViewModels.AggregatedCharts;
    using ViewModels.AggregatedCharts.Histogram;
    using ViewModels.LoadedLapCache;

    public class RpmHistogramProvider : IAggregatedChartProvider
    {
        private readonly ILoadedLapsCache _loadedLapsCache;
        private readonly RpmHistogramDataExtractor _rpmHistogramDataExtractor;
        private readonly IViewModelFactory _viewModelFactory;
        public string ChartName => "RPM Histogram";
        public AggregatedChartKind Kind => AggregatedChartKind.Histogram;

        public RpmHistogramProvider(ILoadedLapsCache loadedLapsCache, RpmHistogramDataExtractor rpmHistogramDataExtractor, IViewModelFactory viewModelFactory)
        {
            _loadedLapsCache = loadedLapsCache;
            _rpmHistogramDataExtractor = rpmHistogramDataExtractor;
            _viewModelFactory = viewModelFactory;
        }

        public IAggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            IReadOnlyCollection<LapTelemetryDto> loadedLaps = _loadedLapsCache.LoadedLaps;
            string title = $"{ChartName} - Laps: {string.Join(", ", loadedLaps.Select(x => x.LapSummary.CustomDisplayName))}";

            int maxGear = loadedLaps.SelectMany(x => x.TimedTelemetrySnapshots).Where(x => !string.IsNullOrWhiteSpace(x.PlayerData.CarInfo.CurrentGear) && x.PlayerData.CarInfo.CurrentGear != "R" && x.PlayerData.CarInfo.CurrentGear != "N").Max(x => int.Parse(x.PlayerData.CarInfo.CurrentGear));

            CompositeAggregatedChartsViewModel viewModel = new CompositeAggregatedChartsViewModel() { Title = title };

            HistogramChartViewModel mainViewModel = _viewModelFactory.Create<HistogramChartViewModel>();
            mainViewModel.FromModel(CreateHistogramAllGears(loadedLaps, _rpmHistogramDataExtractor.DefaultBandSize));

            viewModel.MainAggregatedChartViewModel = mainViewModel;

            for (int i = 1; i <= maxGear; i++)
            {
                Histogram histogram = CreateHistogram(loadedLaps, i, _rpmHistogramDataExtractor.DefaultBandSize);
                if (histogram == null)
                {
                    continue;
                }
                HistogramChartViewModel child = _viewModelFactory.Create<HistogramChartViewModel>();
                child.FromModel(histogram);
                viewModel.AddChildAggregatedChildViewModel(child);
            }

            return viewModel;
        }

        protected Histogram CreateHistogram(IReadOnlyCollection<LapTelemetryDto> loadedLaps, int gear, double bandSize)
        {
            return _rpmHistogramDataExtractor.ExtractSeriesForGear(loadedLaps, bandSize, gear.ToString());
        }

        protected Histogram CreateHistogramAllGears(IReadOnlyCollection<LapTelemetryDto> loadedLaps, double bandSize)
        {
            return _rpmHistogramDataExtractor.ExtractSeriesForAllGear(loadedLaps, bandSize);
        }
    }
}