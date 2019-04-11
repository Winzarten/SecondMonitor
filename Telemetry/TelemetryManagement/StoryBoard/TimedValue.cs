namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System;
    using DataModel.Telemetry;

    public struct TimedValue
    {
        public TimedValue(double value, TimedTelemetrySnapshot startSnapshot, TimedTelemetrySnapshot endSnapshot)
        {
            Value = value;
            StartSnapshot = startSnapshot;
            EndSnapshot = endSnapshot;
            ValueTime = endSnapshot.LapTime - startSnapshot.LapTime;
        }

        public double Value { get; }
        public TimedTelemetrySnapshot StartSnapshot { get; }
        public TimedTelemetrySnapshot EndSnapshot { get; }
        public TimeSpan ValueTime { get; }
    }
}