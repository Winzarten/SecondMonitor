namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;

    public class TelemetryFrame
    {

        public TelemetryFrame(TimedTelemetrySnapshot telemetrySnapshot)
        {
            TelemetrySnapshot = telemetrySnapshot;
        }

        public TimeSpan FrameTime => TelemetrySnapshot.LapTime;
        public Distance FrameDistance => Distance.FromMeters(TelemetrySnapshot.PlayerData.LapDistance);

        public TimedTelemetrySnapshot TelemetrySnapshot { get; }
        public TelemetryFrame PreviousFrame { get; internal set; }
        public TelemetryFrame NextFrame { get; internal set; }


    }
}