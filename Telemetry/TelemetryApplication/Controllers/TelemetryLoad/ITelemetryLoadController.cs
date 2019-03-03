namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TelemetryManagement.DTO;

    public interface ITelemetryLoadController
    {
        Task<IReadOnlyCollection<SessionInfoDto>> GetAllRecentSessionInfoAsync();
        Task<IReadOnlyCollection<SessionInfoDto>> GetAllArchivedSessionInfoAsync();

        Task<SessionInfoDto> LoadRecentSessionAsync(string sessionIdentifier);
        Task<SessionInfoDto> LoadRecentSessionAsync(SessionInfoDto sessionInfoDto);
        Task<SessionInfoDto> AddRecentSessionAsync(SessionInfoDto sessionInfoDto);
        Task<SessionInfoDto> LoadLastSessionAsync();

        Task<LapTelemetryDto> LoadLap(LapSummaryDto lapSummaryDto);

        Task UnloadLap(LapSummaryDto lapSummaryDto);
        Task ArchiveSession(SessionInfoDto sessionInfoDto);
        Task OpenSessionFolder(SessionInfoDto sessionInfoDto);
        void DeleteSession(SessionInfoDto sessionInfoDto);
    }
}