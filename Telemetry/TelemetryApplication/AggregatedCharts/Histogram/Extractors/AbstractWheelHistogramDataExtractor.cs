namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Histogram.Extractors
{
    using System;
    using System.Collections.Generic;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using SecondMonitor.ViewModels.Settings;
    using Settings;
    using TelemetryManagement.DTO;

    public abstract class AbstractWheelHistogramDataExtractor : AbstractHistogramDataExtractor
    {
        protected AbstractWheelHistogramDataExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        protected abstract Func<WheelInfo, double> WheelValueExtractor { get; }

        public Histogram ExtractHistogramFrontLeft(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.FrontLeft);
            return ExtractHistogram(loadedLaps, ExtractFunc, bandSize, "Front Left");
        }

        public Histogram ExtractHistogramFrontRight(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.FrontRight);
            return ExtractHistogram(loadedLaps, ExtractFunc, bandSize, "Front Right");
        }

        public Histogram ExtractHistogramRearLeft(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.RearLeft);
            return ExtractHistogram(loadedLaps, ExtractFunc, bandSize, "Rear Left");
        }

        public Histogram ExtractHistogramRearRight(IEnumerable<LapTelemetryDto> loadedLaps, double bandSize)
        {
            double ExtractFunc(TimedTelemetrySnapshot x) => WheelValueExtractor(x.PlayerData.CarInfo.WheelsInfo.RearRight);
            return ExtractHistogram(loadedLaps, ExtractFunc, bandSize, "Rear Right");
        }
    }
}