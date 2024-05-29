namespace SecondMonitor.ViewModels.CarStatus
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    public class SessionRemainingCalculator
    {
        private readonly IPaceProvider _paceProvider;
        private int _leaderTimeoutLap;

        public SessionRemainingCalculator(IPaceProvider paceProvider)
        {
            _paceProvider = paceProvider;
            _leaderTimeoutLap = -1;
        }

        public TimeSpan GetTimeRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time || dataSet.SessionInfo.SessionLengthType == SessionLengthType.TimeWithExtraLap)
            {
                return TimeSpan.FromSeconds(dataSet.SessionInfo.SessionTimeRemaining);
            }
            else
            {
                return _paceProvider.LeadersPace != null
                    ? TimeSpan.FromSeconds(GetLeaderLapsToGo(dataSet) * _paceProvider.LeadersPace.Value.TotalSeconds)
                    : TimeSpan.Zero;
            }
        }

        public void Reset()
        {
            _leaderTimeoutLap = -1;
        }

        public double GetLapsRemaining(SimulatorDataSet dataSet)
        {
            TimeSpan? playerPace = _paceProvider.PlayersPace;
            TimeSpan? leaderPace = _paceProvider.LeadersPace;

            if (playerPace == null || leaderPace == null || playerPace.Value == TimeSpan.Zero || leaderPace.Value == TimeSpan.Zero)
            {
                return double.NaN;
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                return GetLeaderLapsToGo(dataSet);
            }
            else
            {
                if (_leaderTimeoutLap == -1 && dataSet.SessionInfo.SessionTimeRemaining <= 0)
                {
                    _leaderTimeoutLap = dataSet.LeaderInfo.CompletedLaps;
                }
                double secondsTillSessionEnds = dataSet.LeaderInfo.IsPlayer ? dataSet.SessionInfo.SessionTimeRemaining : GetSecondsTillLeaderFinished(dataSet, leaderPace.Value);
                double distanceToGo = (secondsTillSessionEnds / playerPace.Value.TotalSeconds) * dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
                double distanceWithLapDistance = distanceToGo + dataSet.PlayerInfo.LapDistance;
                double distanceToFinishLap = dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters - ((int)distanceWithLapDistance % (int)dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
                double totalDistanceToGo = distanceToGo + distanceToFinishLap;

                if (dataSet.LeaderInfo.FinishStatus == DriverFinishStatus.Finished)
                {
                    totalDistanceToGo -= dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
                }

                if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.TimeWithExtraLap && (_leaderTimeoutLap == -1 || _leaderTimeoutLap == dataSet.LeaderInfo.CompletedLaps))
                {
                    totalDistanceToGo += dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
                }

                return totalDistanceToGo / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            }
        }

        private double GetSecondsTillLeaderFinished(SimulatorDataSet dataSet, TimeSpan leaderPace)
        {
            double distanceToGo = (dataSet.SessionInfo.SessionTimeRemaining /
                                   leaderPace.TotalSeconds) * dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            double distanceWithLapDistance = distanceToGo + dataSet.LeaderInfo.LapDistance;
            double distanceToFinishLap = dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters - ((int)distanceWithLapDistance % (int)dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
            double totalDistanceToGo = distanceToGo + distanceToFinishLap;
            double totalLapsToGo = totalDistanceToGo / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            return totalLapsToGo * leaderPace.TotalSeconds;
        }

        private double GetLeaderLapsToGo(SimulatorDataSet dataSet)
        {
            double fullLapsToGo = dataSet.SessionInfo.TotalNumberOfLaps - dataSet.SessionInfo.LeaderCurrentLap + 1;
            return fullLapsToGo - (dataSet.LeaderInfo.LapDistance / dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
        }
    }
}