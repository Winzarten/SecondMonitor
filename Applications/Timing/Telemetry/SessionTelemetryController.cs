namespace SecondMonitor.Timing.Telemetry
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel.BasicProperties;
    using NLog;
    using SecondMonitor.Telemetry.TelemetryManagement.DTO;
    using SecondMonitor.Telemetry.TelemetryManagement.Repository;
    using SessionTiming.Drivers.ViewModel;

    public class SessionTelemetryController : ISessionTelemetryController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITelemetryRepository _telemetryRepository;
        private bool _sessionInfoSaved;

        public SessionTelemetryController(string trackName, SessionType sessionType, ITelemetryRepository telemetryRepository)
        {
            _telemetryRepository = telemetryRepository;
            SessionIdentifier = $"{DateTime.Now:yy-MM-dd-hh-mm}-{trackName}-{sessionType}";
        }

        public string SessionIdentifier { get; }

        public Task SaveLapTelemetry(LapInfo lapInfo)
        {
            Logger.Info($"Saving Telemetry for Lap:{lapInfo.LapNumber}");
            if (lapInfo.LapTelemetryInfo.IsPurged)
            {
                Logger.Error("Lap Is PURGED! Cannot Save");
                return Task.CompletedTask;
            }

            if (!lapInfo.Valid)
            {
                Logger.Error("Lap Is Invalid! Cannot Save");
                return Task.CompletedTask;
            }

            return Task.Run(() => SaveLapTelemetrySync(lapInfo));
        }

        private void SaveLapTelemetrySync(LapInfo lapInfo)
        {
            if (!_sessionInfoSaved)
            {
                SaveSessionInfo(lapInfo);
            }

            SaveLap(lapInfo);
        }

        private void SaveLap(LapInfo lapInfo)
        {
            LapTelemetryDto lapTelemetryDto = new LapTelemetryDto()
            {
                LapNumber = lapInfo.LapNumber,
                LapTimeSeconds = lapInfo.LapTime.TotalSeconds,
                TimedTelemetrySnapshots = lapInfo.LapTelemetryInfo.TimedTelemetrySnapshots.Snapshots.ToArray()
            };
            _telemetryRepository.SaveSessionLap(lapTelemetryDto, SessionIdentifier);
        }

        private void SaveSessionInfo(LapInfo lapInfo)
        {
            SessionInfoDto sessionInfoDto = new SessionInfoDto()
            {
                CarName = lapInfo.Driver.CarName,
                TrackName = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackName,
                LayoutName = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackLayoutName,
                LayoutLength = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters,
                PlayerName = lapInfo.Driver.Name,
                Simulator = lapInfo.Driver.Session.LastSet.Source,
            };
            _telemetryRepository.SaveSessionInformation(sessionInfoDto, SessionIdentifier);
            _sessionInfoSaved = true;
        }
    }
}