namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Threading.Tasks;
    using TelemetryManagement.DTO;

    public interface ITelemetryLoadController
    {
        Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier);
        Task<SessionInfoDto> LoadLastSessionAsync();

        Task<LapTelemetryDto> LoadLap(int lapNumber);

        Task UnloadLap(LapSummaryDto lapSummaryDto);

    }
}