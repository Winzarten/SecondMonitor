namespace SecondMonitor.ViewModels.CarStatus
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class SessionRemainingCalculator
    {
        private readonly IPaceProvider _paceProvider;

        public SessionRemainingCalculator(IPaceProvider paceProvider)
        {
            _paceProvider = paceProvider;
        }

        public TimeSpan GetTimeRemaining(SimulatorDataSet dataSet)
        {
            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Time)
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

        public double GetLapsRemaining(SimulatorDataSet dataSet)
        {
            TimeSpan? playerPace = _paceProvider.PlayersPace;
            TimeSpan? leaderPace = _paceProvider.LeadersPace;

            if (playerPace == null || leaderPace == null)
            {
                return double.NaN;
            }

            if (dataSet.SessionInfo.SessionLengthType == SessionLengthType.Laps)
            {
                return GetLeaderLapsToGo(dataSet);
            }
            else
            {
                double secondsTillSessionEnds = dataSet.LeaderInfo.IsPlayer ? dataSet.SessionInfo.SessionTimeRemaining : GetSecondsTillLeaderFinished(dataSet, leaderPace.Value);
                double distanceToGo = (secondsTillSessionEnds / playerPace.Value.TotalSeconds) * dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
                double distanceWithLapDistance = distanceToGo + dataSet.PlayerInfo.LapDistance;
                double distanceToFinishLap = dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters - ((int)distanceWithLapDistance % (int)dataSet.SessionInfo.TrackInfo.LayoutLength.InMeters);
                double totalDistanceToGo = distanceToGo + distanceToFinishLap;
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