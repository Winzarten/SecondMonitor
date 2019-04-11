namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using Settings;
    using TelemetryManagement.StoryBoard;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractHistogramDataExtractor : AbstractTelemetryDataExtractor
    {
        protected abstract bool ZeroBandInMiddle { get; }
        protected abstract double DefaultBandSize { get; }
        protected abstract string Unit { get; }

        protected AbstractHistogramDataExtractor(ISettingsProvider settingsProvider, ILoadedLapsCache loadedLapsCache) : base(settingsProvider, loadedLapsCache)
        {
        }

        protected Histogram ExtractHistogram(Func<TimedTelemetrySnapshot, double> extractFunc, double bandSize, string title)
        {
            TimedValue[] data = ExtractTimedValuesOfLoadedLaps(extractFunc).OrderBy(x => x.Value).ToArray();
            double minBand = GetBandMiddleValue(data[0].Value, bandSize);
            double maxBand = GetBandMiddleValue(data[data.Length - 1].Value, bandSize);
            double totalSeconds = data.Sum(x => x.ValueTime.TotalSeconds);
            List<IGrouping<double, TimedValue>> groupedByBand = data.GroupBy(x => GetBandMiddleValue(x.Value, bandSize)).ToList();

            Histogram histogram = new Histogram()
            {
                BandSize = bandSize,
                Title = title,
                Unit = Unit,
            };
            for (double i = minBand; i <= maxBand; i += bandSize)
            {
                IGrouping<double, TimedValue> currentGrouping = groupedByBand.FirstOrDefault(x => x.Key == i);

                string category = i.ToStringScalableDecimals();
                double bandTime = currentGrouping?.Sum(x => x.ValueTime.TotalSeconds) ?? 0;
                double percentage = bandTime / totalSeconds * 100;
                HistogramBand currentBand = new HistogramBand(currentGrouping?.ToArray() ?? new TimedValue[0], category, percentage);
                histogram.AddItem(currentBand);
            }

            return histogram;
        }

        protected double GetBandMiddleValue(double value, double bandSize)
        {
            double coef = value < 0 ? -bandSize / 2 : bandSize / 2;
            //Computes the middle value of the band the value belongs to
            return ZeroBandInMiddle ? ((int) ((value + coef) / bandSize)) * bandSize : ((int)(value / bandSize)) * bandSize + coef;
        }
    }
}