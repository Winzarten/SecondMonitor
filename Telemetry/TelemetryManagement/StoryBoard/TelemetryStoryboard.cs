namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using DTO;

    public class TelemetryStoryboard
    {
        public TelemetryStoryboard(LapSummaryDto lapSummaryDto)
        {
            LapSummaryDto = lapSummaryDto;
            TelemetryFrames = new List<TelemetryFrame>().AsReadOnly();
        }

        public LapSummaryDto LapSummaryDto { get; }
        public TelemetryFrame FirstFrame { get; internal set; }
        public TelemetryFrame LastFrame { get; internal set; }
        public IReadOnlyCollection<TelemetryFrame> TelemetryFrames { get; internal set; }

        public TelemetryFrame FindFrameByTime(TimeSpan time)
        {
            TelemetryFrame frame = TelemetryFrames.FirstOrDefault(x => x.FrameTime >= time);
            return frame ?? LastFrame;
        }

        public TelemetryFrame FindFrameByDistance(Distance distance)
        {
            TelemetryFrame frame = TelemetryFrames.FirstOrDefault(x => x.FrameDistance.InMeters >= distance.InMeters);
            return frame ?? LastFrame;
        }

        public double GetValueByTime(TimeSpan time, Func<TimedTelemetrySnapshot, double> valueFunc)
        {
            TelemetryFrame closestFrame = FindFrameByTime(time);
            TelemetryFrame previousFrame = closestFrame.PreviousFrame;
            return valueFunc(closestFrame.TelemetrySnapshot);
        }

        public double GetValueByDistance(Distance distance, Func<TimedTelemetrySnapshot, double> valueFunc)
        {
            TelemetryFrame closestFrame = FindFrameByDistance(distance);
            TelemetryFrame previousFrame = closestFrame.PreviousFrame;
            if (previousFrame == null)
            {
                return valueFunc(closestFrame.TelemetrySnapshot);
            }

            double y0 = valueFunc(previousFrame.TelemetrySnapshot);
            double y1 = valueFunc(closestFrame.TelemetrySnapshot);

            double x0 = previousFrame.FrameDistance.InMeters;
            double x1 = closestFrame.FrameDistance.InMeters;

            double rate = (y1 - y0) / (x1 - x0);

            double distanceFromX = distance.InMeters - x0;

            return y0 + (rate * distanceFromX);
        }
    }
}