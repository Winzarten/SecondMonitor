namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;

    public class TelemetryFrame
    {

        public TelemetryFrame(TimedTelemetrySnapshot telemetrySnapshot, TelemetryStoryboard storyboard)
        {
            TelemetrySnapshot = telemetrySnapshot;
            Storyboard = storyboard;
        }

        public TimeSpan FrameTime => TelemetrySnapshot.LapTime;
        public Distance FrameDistance => Distance.FromMeters(TelemetrySnapshot.PlayerData.LapDistance);

        public TimedTelemetrySnapshot TelemetrySnapshot { get; }
        public TelemetryStoryboard Storyboard { get; }
        public TelemetryFrame PreviousFrame { get; internal set; }
        public TelemetryFrame NextFrame { get; internal set; }


        public TelemetryFrame Forward(TimeSpan timeSpan)
        {
            TimeSpan toFind = timeSpan + TelemetrySnapshot.LapTime;
            TelemetryFrame currentFrame = this;
            while (currentFrame != null && currentFrame.FrameTime < toFind)
            {
                currentFrame = currentFrame.NextFrame;
            }

            return currentFrame ?? Storyboard.LastFrame;
        }

    }
}