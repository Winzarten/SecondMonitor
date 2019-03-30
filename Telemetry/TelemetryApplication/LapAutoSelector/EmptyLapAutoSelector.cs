namespace SecondMonitor.Telemetry.TelemetryApplication.LapAutoSelector
{
    using System.Collections.Generic;
    using TelemetryManagement.DTO;

    public class EmptyLapAutoSelector : ILapAutoSelector
    {
        public string SelectorName => "None";

        public bool TryPickLaps(IEnumerable<LapSummaryDto> laps, out IReadOnlyCollection<LapSummaryDto> selectedLaps, out LapSummaryDto referenceLap)
        {
            selectedLaps = null;
            referenceLap = null;
            return false;
        }
    }
}