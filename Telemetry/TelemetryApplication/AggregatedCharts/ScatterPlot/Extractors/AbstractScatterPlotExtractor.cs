namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using OxyPlot;
    using Settings;
    using TelemetryManagement.DTO;

    public abstract class AbstractScatterPlotExtractor : AbstractTelemetryDataExtractor
    {
        protected AbstractScatterPlotExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public abstract string YUnit { get; }
        public abstract string XUnit { get; }
        public abstract double XMajorTickSize { get; }
        public abstract double YMajorTickSize { get; }

        protected ScatterPlotSeries ExtractSeries(IEnumerable<LapTelemetryDto> loadedLaps, Predicate<TimedTelemetrySnapshot> filter, string seriesTitle, OxyColor color)
        {
            TimedTelemetrySnapshot[] timedTelemetrySnapshots = loadedLaps.SelectMany(x => x.TimedTelemetrySnapshots).Where(x => filter(x)).ToArray();

            if (timedTelemetrySnapshots.Length == 0)
            {
                return null;
            }

            ScatterPlotSeries newSeries = new ScatterPlotSeries(color, seriesTitle);
            timedTelemetrySnapshots.ForEach(x => newSeries.AddDataPoint(GetXValue(x), GetYValue(x)));
            return newSeries;
        }

        protected abstract double GetXValue(TimedTelemetrySnapshot snapshot);
        protected abstract double GetYValue(TimedTelemetrySnapshot snapshot);

    }
}