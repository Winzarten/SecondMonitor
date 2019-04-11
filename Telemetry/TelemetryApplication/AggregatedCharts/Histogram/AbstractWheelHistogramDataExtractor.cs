namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram
{
    using System;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using Settings;
    using ViewModels.LoadedLapCache;

    public abstract class AbstractWheelHistogramDataExtractor : AbstractHistogramDataExtractor
    {
        protected AbstractWheelHistogramDataExtractor(ISettingsProvider settingsProvider, ILoadedLapsCache loadedLapsCache) : base(settingsProvider, loadedLapsCache)
        {
        }

        protected abstract Func<WheelInfo, double> WheelValueExtractor { get; }

        public Histogram ExtractHistogramFrontLeft() => ExtractHistogramFrontLeft(DefaultBandSize);


        public Histogram ExtractHistogramFrontLeft(double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.FrontLeft);
            return ExtractHistogram(ExtractFunc, bandSize, "Front Left");
        }

        public Histogram ExtractHistogramFrontRight() => ExtractHistogramFrontRight(DefaultBandSize);

        public Histogram ExtractHistogramFrontRight(double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.FrontRight);
            return ExtractHistogram(ExtractFunc, bandSize, "Front Right");
        }

        public Histogram ExtractHistogramRearLeft() => ExtractHistogramRearLeft(DefaultBandSize);

        public Histogram ExtractHistogramRearLeft(double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.RearLeft);
            return ExtractHistogram(ExtractFunc, bandSize, "Rear Left");
        }

        public Histogram ExtractHistogramRearRight() => ExtractHistogramRearRight(DefaultBandSize);

        public Histogram ExtractHistogramRearRight(double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.RearRight);
            return ExtractHistogram(ExtractFunc, bandSize, "Rear Right");
        }
    }
}