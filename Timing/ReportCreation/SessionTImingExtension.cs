namespace SecondMonitor.Timing.ReportCreation
{
    using System;
    using System.Linq;

    using DataModel.Summary;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;

    public static class SessionTimingExtension
    {
        public static SessionSummary ToSessionSummary(this SessionTiming timing)
        {
            SessionSummary summary = new SessionSummary();
            FillSessionInfo(summary, timing);
            AddDrivers(summary, timing);
            return summary;
        }

        private static void FillSessionInfo(SessionSummary summary, SessionTiming timing)
        {
            summary.SessionType = timing.SessionType;
            summary.TrackInfo = timing.LastSet.SessionInfo.TrackInfo;
            summary.Simulator = timing.LastSet.Source;
            summary.SessionLength = TimeSpan.FromSeconds(timing.TotalSessionLength);
            summary.SessionLengthType = timing.LastSet.SessionInfo.SessionLengthType;
            summary.TotalNumberOfLaps = timing.LastSet.SessionInfo.TotalNumberOfLaps;
        }

        private static void AddDrivers(SessionSummary summary, SessionTiming timing)
        {
           summary.Drivers.AddRange(timing.Drivers.Select(d => ConvertToSummaryDriver(d.Value.DriverTiming, timing.SessionType)));
        }

        private static Driver ConvertToSummaryDriver(DriverTiming driverTiming, SessionType sessionTime)
        {
            Driver driverSummary = new Driver()
                                       {
                                           DriverName = driverTiming.Name,
                                           CarName = driverTiming.CarName,
                                           Finished = driverTiming.IsActive,
                                           FinishingPosition = driverTiming.Position,
                                           TopSpeed = driverTiming.TopSpeed,
                                           IsPlayer = driverTiming.DriverInfo.IsPlayer
                                       };
            int lapNumber = 1;
            bool allLaps = sessionTime == SessionType.Race;
            driverSummary.Laps.AddRange(driverTiming.Laps.Where(l => l.Completed && (allLaps || l.Valid)).Select(l => ConvertToSummaryLap(driverSummary, l, lapNumber++)));
            driverSummary.TotalLaps = driverSummary.Laps.Count;
            return driverSummary;
        }

        private static Lap ConvertToSummaryLap(Driver summaryDriver,  LapInfo lapInfo, int lapNumber)
        {
            Lap summaryLap = new Lap(summaryDriver, lapInfo.Valid)
                                 {
                                     LapNumber = lapNumber,
                                     LapTime = lapInfo.LapTime,
                                     Sector1 = lapInfo.Sector1?.Duration ?? TimeSpan.Zero,
                                     Sector2 = lapInfo.Sector2?.Duration ?? TimeSpan.Zero,
                                     Sector3 = lapInfo.Sector3?.Duration ?? TimeSpan.Zero,
                                     LapEndSnapshot = lapInfo.LapTelemetryInfo.LapEndSnapshot,
                                     LapStartSnapshot = lapInfo.LapTelemetryInfo.LapStarSnapshot
            };
            return summaryLap;
        }
    }
}