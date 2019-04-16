namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using DataModel.Telemetry;
    using Settings;

    public class RpmToHorizontalGExtractor : AbstractGearFilteredScatterPlotExtractor
    {
        public RpmToHorizontalGExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string YUnit => "G";

        public override string XUnit => "RPM";

        public override double XMajorTickSize => 1000;

        public override double YMajorTickSize => 1;

        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.EngineRpm;
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.Acceleration.ZinG;
        }
    }
}