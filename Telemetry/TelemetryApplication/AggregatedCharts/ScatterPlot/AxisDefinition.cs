namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot
{
    public class AxisDefinition
    {
        public AxisDefinition(double majorTick, double minorTick, string unit)
        {
            MajorTick = majorTick;
            MinorTick = minorTick;
            Unit = unit;
        }

        public double MajorTick { get; }
        public double MinorTick { get; }
        public string Unit { get; }
    }
}