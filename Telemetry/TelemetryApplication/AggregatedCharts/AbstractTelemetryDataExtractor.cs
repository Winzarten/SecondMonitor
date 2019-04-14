namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings;
    using TelemetryManagement.DTO;
    using TelemetryManagement.StoryBoard;

    public abstract class AbstractTelemetryDataExtractor
    {
        protected AbstractTelemetryDataExtractor(ISettingsProvider settingsProvider)
        {
            VelocityUnitsSmall = settingsProvider.DisplaySettingsViewModel.VelocityUnitsVerySmall;
            VelocityUnits = settingsProvider.DisplaySettingsViewModel.VelocityUnits;
        }

        protected VelocityUnits VelocityUnits { get; }
        protected VelocityUnits VelocityUnitsSmall { get; }

        protected TimedValue[] ExtractTimedValuesOfLoadedLaps(IEnumerable<LapTelemetryDto> loadedLaps, Func<TimedTelemetrySnapshot, double> extractionFunc)
        {
            return ExtractTimedValuesOfLoadedLaps(loadedLaps, extractionFunc, (x) => true);
        }

        protected TimedValue[] ExtractTimedValuesOfLoadedLaps(IEnumerable<LapTelemetryDto> loadedLaps, Func<TimedTelemetrySnapshot, double> extractionFunc, Predicate<TimedTelemetrySnapshot> filter)
        {
            IEnumerable<TimedValue> timedValues = loadedLaps.SelectMany(x => ExtractTimedValues(x.TimedTelemetrySnapshots, extractionFunc, filter));
            return timedValues.ToArray();
        }

        private static List<TimedValue> ExtractTimedValues(TimedTelemetrySnapshot[] snapshots, Func<TimedTelemetrySnapshot, double> extractDataFunc, Predicate<TimedTelemetrySnapshot> filter)
        {
            List<TimedValue> timedValues = new List<TimedValue>(snapshots.Length);
            for (int i = 0; i < snapshots.Length - 1; i++)
            {
                TimedTelemetrySnapshot firstSnapshot = snapshots[i];
                TimedTelemetrySnapshot secondSnapshot = snapshots[i + 1];
                double value = (extractDataFunc(firstSnapshot) + extractDataFunc(secondSnapshot)) / 2;

                timedValues.Add(new TimedValue(value, firstSnapshot, secondSnapshot));
            }

            return timedValues;
        }
    }
}