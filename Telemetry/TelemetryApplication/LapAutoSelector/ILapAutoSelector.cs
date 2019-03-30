namespace SecondMonitor.Telemetry.TelemetryApplication.LapAutoSelector
{
    using System.Collections.Generic;
    using TelemetryManagement.DTO;

    public interface ILapAutoSelector
    {
        string SelectorName { get; }

        bool TryPickLaps(IEnumerable<LapSummaryDto> laps, out IReadOnlyCollection<LapSummaryDto> selectedLaps, out LapSummaryDto referenceLap);
    }
}