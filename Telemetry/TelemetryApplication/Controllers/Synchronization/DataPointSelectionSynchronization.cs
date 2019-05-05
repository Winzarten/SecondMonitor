namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Extensions;
    using TelemetryManagement.StoryBoard;

    public class DataPointSelectionSynchronization : IDataPointSelectionSynchronization
    {
        public event EventHandler<TimedValuesArgs> OnPointsSelected;
        public event EventHandler<TimedValuesArgs> OnPointsDeselected;

        private readonly Dictionary<TimedValue, int> _selectionCount;

        public DataPointSelectionSynchronization()
        {
            _selectionCount = new Dictionary<TimedValue, int>();
        }

        public void SelectPoints(IReadOnlyCollection<TimedValue> timedValues)
        {
            IReadOnlyCollection<TimedValue> firstTimeSelected = timedValues.Except(_selectionCount.Keys).ToList();
            firstTimeSelected.ForEach(x => _selectionCount[x] = 0);
            timedValues.ForEach(x => _selectionCount[x]++);
            OnPointsSelected?.Invoke(this, new TimedValuesArgs(firstTimeSelected));
        }

        public void DeSelectPoints(IReadOnlyCollection<TimedValue> timedValues)
        {
            timedValues.ForEach(x => _selectionCount[x]--);
            IReadOnlyCollection<TimedValue> reallyDeselected = _selectionCount.Where(x => x.Value == 0).Select(x => x.Key).ToList();
            reallyDeselected.ForEach(x => _selectionCount.Remove(x));
            OnPointsDeselected?.Invoke(this, new TimedValuesArgs(reallyDeselected));
        }
    }
}