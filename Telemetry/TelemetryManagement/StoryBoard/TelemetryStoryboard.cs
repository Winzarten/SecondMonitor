namespace SecondMonitor.Telemetry.TelemetryManagement.StoryBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.BasicProperties;
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
    }
}