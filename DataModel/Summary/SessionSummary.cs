﻿namespace SecondMonitor.DataModel.Summary
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;

    public class SessionSummary
    {

        public DateTime SessionRunTime { get; set; } = DateTime.Now;

        public SessionType SessionType { get; set; }

        public SessionPhase SessionPhase { get; set; }

        public SessionLengthType SessionLengthType { get; set; } = SessionLengthType.Na;

        public int TotalNumberOfLaps { get; set; } = 0;

        public TimeSpan SessionLength { get; set; }

        public TrackInfo TrackInfo { get; set; } = new TrackInfo();

    }
}