namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TelemetryManagement.DTO;

    public interface ITelemetryLoadController
    {
        Task<IReadOnlyCollection<SessionInfoDto>> GetAllRecentSessionInfoAsync();
        Task<SessionInfoDto> LoadRecentSessionAsync(string sessionIdentifier);
        Task<SessionInfoDto> LoadRecentSessionAsync(SessionInfoDto sessionInfoDto);
        Task<SessionInfoDto> LoadLastSessionAsync();

        Task<LapTelemetryDto> LoadLap(LapSummaryDto lapSummaryDto);

        Task UnloadLap(LapSummaryDto lapSummaryDto);

    }
}