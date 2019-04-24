namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Collections.Generic;
    using TelemetryManagement.StoryBoard;

    public class TimedValuesArgs : EventArgs
    {
        public TimedValuesArgs(IReadOnlyCollection<TimedValue> timedValues)
        {
            TimedValues = timedValues;
        }

        public IReadOnlyCollection<TimedValue> TimedValues { get; }
    }
}