namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using System.Collections.Generic;
    using BasicProperties;

    [Serializable]
    public sealed class SessionInfo
    {
        public SessionInfo()
        {
            SessionTime = TimeSpan.Zero;
            TrackInfo = new TrackInfo();
        }

        public TimeSpan SessionTime { get; set; }

        public TrackInfo TrackInfo { get; set; }

        public bool IsActive { get; set; }

        public SessionType SessionType { get; set; }

        public SessionPhase SessionPhase { get; set; }

        public SessionLengthType SessionLengthType { get; set; } = SessionLengthType.Na;

        public double SessionTimeRemaining { get; set; } = 0;

        public int TotalNumberOfLaps { get; set; } = 0;

        public int LeaderCurrentLap { get; set; }

        public WeatherInfo WeatherInfo { get; set; } = new WeatherInfo();

        public List<FlagKind> ActiveFlags = new List<FlagKind>();

        public bool IsMultiClass { get; set; }

        public bool IsMultiplayer { get; set; } = false;
    }
}
