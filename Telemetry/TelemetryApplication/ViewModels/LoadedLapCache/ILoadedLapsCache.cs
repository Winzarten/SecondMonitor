namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LoadedLapCache
{
    using System.Collections.Generic;
    using TelemetryManagement.DTO;

    public interface ILoadedLapsCache
    {
        IReadOnlyCollection<LapTelemetryDto> LoadedLaps { get; }

        LapSummaryDto ReferenceLap { get;  }
    }
}