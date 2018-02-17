namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    public class SessionInfo
    {
        public enum SessionTypeEnum { Na, Practice, Qualification, WarmUp, Race}

        public enum SessionPhaseEnum { Countdown, Green, Checkered}

        public enum SessionLengthTypeEnum { Na, Laps, Time}

        public TimeSpan SessionTime { get; set; }

        public bool IsActive { get; set; }

        public string TrackName { get; set; }

        public string TrackLayoutName { get; set; }

        public float LayoutLength { get; set; }


        public SessionTypeEnum SessionType { get; set; }

        public SessionPhaseEnum SessionPhase { get; set; }

        public SessionLengthTypeEnum SessionLengthType { get; set; } = SessionLengthTypeEnum.Na;


        public float SessionTimeRemaining { get; set; } = 0;

        public int TotalNumberOfLaps { get; set; } = 0;

        public int LeaderCurrentLap { get; set; }

        public WeatherInfo WeatherInfo { get; set; } = new WeatherInfo();

        public SessionInfo()
        {
            this.SessionTime = new TimeSpan(0);
            this.TrackName = string.Empty;
            this.TrackLayoutName = string.Empty;
        }
    }
}
