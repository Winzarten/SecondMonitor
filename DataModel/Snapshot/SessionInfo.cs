﻿namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;

    public class SessionInfo
    {
        public TimeSpan SessionTime { get; set; }

        public TrackInfo TrackInfo { get; set; }

        public bool IsActive { get; set; }

        public SessionType SessionType { get; set; }

        public SessionPhase SessionPhase { get; set; }

        public SessionLengthType SessionLengthType { get; set; } = SessionLengthType.Na;

        public float SessionTimeRemaining { get; set; } = 0;

        public int TotalNumberOfLaps { get; set; } = 0;

        public int LeaderCurrentLap { get; set; }

        public WeatherInfo WeatherInfo { get; set; } = new WeatherInfo();

        public SessionInfo()
        {
            this.SessionTime = TimeSpan.Zero;
            this.TrackInfo = new TrackInfo();
        }
    }
}
