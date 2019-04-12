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
        }

        protected VelocityUnits VelocityUnitsSmall { get; set; }

        public TimedValue[] ExtractTimedValuesOfLoadedLaps(IEnumerable<LapTelemetryDto> loadedLaps, Func<TimedTelemetrySnapshot, double> extractionFunc)
        {
            IEnumerable<TimedValue> timedValues = loadedLaps.SelectMany(x => ExtractTimedValues(x.TimedTelemetrySnapshots, extractionFunc));
            return timedValues.ToArray();
        }

        private static List<TimedValue> ExtractTimedValues(TimedTelemetrySnapshot[] snapshots, Func<TimedTelemetrySnapshot, double> extractDataFunc)
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