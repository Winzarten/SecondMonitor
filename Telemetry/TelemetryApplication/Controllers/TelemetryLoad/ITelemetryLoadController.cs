namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.TelemetryLoad
{
    using System.Threading.Tasks;
    using TelemetryManagement.DTO;

    public interface ITelemetryLoadController
    {
        Task<SessionInfoDto> LoadSessionAsync(string sessionIdentifier);
    }
}