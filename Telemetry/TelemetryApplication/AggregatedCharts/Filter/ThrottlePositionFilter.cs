namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using System;
    using DataModel.Telemetry;

    public class ThrottlePositionFilter : ITelemetryFilter
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }

        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            double pedalPos = Math.Abs(dataSet.InputInfo.ThrottlePedalPosition);
            return pedalPos >= Minimum && pedalPos < Maximum;
        }
    }
}