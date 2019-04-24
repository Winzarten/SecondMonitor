namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Collections.Generic;
    using TelemetryManagement.StoryBoard;

    public interface IDataPointSelectionSynchronization
    {
        event EventHandler<TimedValuesArgs> OnPointsSelected;
        event EventHandler<TimedValuesArgs> OnPointsDeselected;

        void SelectPoints(IReadOnlyCollection<TimedValue> timedValues);
        void DeSelectPoints(IReadOnlyCollection<TimedValue> timedValues);
    }
}