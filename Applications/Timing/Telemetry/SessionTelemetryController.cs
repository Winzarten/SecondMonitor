﻿namespace SecondMonitor.Timing.Telemetry
{
    using System;
    using System.Collections.Generic;
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
        private SessionInfoDto _sessionInfoDto;

        public SessionTelemetryController(string trackName, SessionType sessionType, ITelemetryRepository telemetryRepository)
        {
            _telemetryRepository = telemetryRepository;
            SessionIdentifier = $"{DateTime.Now:yy-MM-dd-hh-mm}-{trackName}-{sessionType}";
        }

        public string SessionIdentifier { get; }

        public Task<bool> TrySaveLapTelemetry(LapInfo lapInfo)
        {
            Logger.Info($"Saving Telemetry for Lap:{lapInfo.LapNumber}");
            if (lapInfo.LapTelemetryInfo.IsPurged)
            {
                Logger.Error("Lap Is PURGED! Cannot Save");
                return Task.FromResult(false);
            }

            if (!lapInfo.Valid)
            {
                Logger.Error("Lap Is Invalid! Cannot Save");
                return Task.FromResult(false);
            }

            return Task.Run(() => SaveLapTelemetrySync(lapInfo));
        }

        private bool SaveLapTelemetrySync(LapInfo lapInfo)
        {
            if (_sessionInfoDto == null)
            {
                _sessionInfoDto = CreateSessionInfo(lapInfo);
            }

            return TrySaveLap(lapInfo);
        }

        private bool TrySaveLap(LapInfo lapInfo)
        {
            try
            {
                LapSummaryDto lapSummaryDto = new LapSummaryDto()
                {
                    LapNumber = lapInfo.LapNumber,
                    LapTimeSeconds = lapInfo.LapTime.TotalSeconds
                };

                LapTelemetryDto lapTelemetryDto = new LapTelemetryDto()
                {
                    LapSummary = lapSummaryDto,
                    TimedTelemetrySnapshots = lapInfo.LapTelemetryInfo.TimedTelemetrySnapshots.Snapshots.ToArray()
                };
                _sessionInfoDto.LapsSummary.Add(lapSummaryDto);

                _telemetryRepository.SaveSessionInformation(_sessionInfoDto, SessionIdentifier);
                _telemetryRepository.SaveSessionLap(lapTelemetryDto, SessionIdentifier);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Uanble to Save Telemetry");
                return false;
            }
        }

        private SessionInfoDto CreateSessionInfo(LapInfo lapInfo)
        {
            SessionInfoDto sessionInfoDto = new SessionInfoDto()
            {
                CarName = lapInfo.Driver.CarName,
                TrackName = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackName,
                LayoutName = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.TrackLayoutName,
                LayoutLength = lapInfo.Driver.Session.LastSet.SessionInfo.TrackInfo.LayoutLength.InMeters,
                PlayerName = lapInfo.Driver.Name,
                Simulator = lapInfo.Driver.Session.LastSet.Source,
                SessionRunDateTime = DateTime.Now,
                LapsSummary = new List<LapSummaryDto>()
            };
            return sessionInfoDto;
        }
    }
}