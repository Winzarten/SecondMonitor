namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System;
    using System.Linq;
    using DataModel.Telemetry;
    using Settings;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractHistogramDataExtractor : AbstractTelemetryDataExtractor
    {
        public bool ZeroBandInMiddle { get; set; }

        protected AbstractHistogramDataExtractor(ISettingsProvider settingsProvider, ILoadedLapsCache loadedLapsCache) : base(settingsProvider, loadedLapsCache)
        {
        }

        protected Histogram ExtractHistogram(Func<TimedTelemetrySnapshot, double> extractFunc, double bandSize)
        {
            double[] data = ExtractData(extractFunc).OrderBy(x => x).ToArray();

        }

        protected double GetBandMiddleValue(double value, double bandSize)
        {
            double coef = value < 0 ? -value / 2 : value / 2;
            //Computes the middle value of the band the value belongs to
            return ZeroBandInMiddle ? ((int) ((value + coef) / bandSize)) * bandSize : ((int)(value / bandSize)) * bandSize + coef;
        }
    }
}