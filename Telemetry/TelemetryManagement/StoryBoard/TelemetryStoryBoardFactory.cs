namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Telemetry;
    using DTO;

    public class TelemetryStoryBoardFactory
    {
        public TelemetryStoryboard Create(LapTelemetryDto lapTelemetryDto)
        {
            TelemetryStoryboard telemetryStoryboard = new TelemetryStoryboard(lapTelemetryDto.LapSummary);
            List<TelemetryFrame> telemetryFrames = new List<TelemetryFrame>();
            TelemetryFrame previousFrame = null;
            foreach (TimedTelemetrySnapshot timedTelemetrySnapshot in lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.LapTimeSeconds))
            {
                TelemetryFrame currentFrame = new TelemetryFrame(timedTelemetrySnapshot) {PreviousFrame = previousFrame};
                if (previousFrame == null)
                {
                    telemetryStoryboard.FirstFrame = currentFrame;
                }
                else
                {
                    previousFrame.NextFrame = currentFrame;
                }
                telemetryFrames.Add(currentFrame);
                previousFrame = currentFrame;
            }

            telemetryStoryboard.TelemetryFrames = telemetryFrames.AsReadOnly();
            telemetryStoryboard.LastFrame = previousFrame;
            return telemetryStoryboard;
        }
    }
}