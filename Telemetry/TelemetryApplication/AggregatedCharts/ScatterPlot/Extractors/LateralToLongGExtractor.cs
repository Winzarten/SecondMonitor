namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using DataModel.Telemetry;
    using SecondMonitor.ViewModels.Settings;

    public class LateralToLongGExtractor : AbstractScatterPlotExtractor
    {
        public LateralToLongGExtractor(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string XUnit => "G";

        public override double XMajorTickSize => 0.5;

        public override string YUnit => "G";

        public override double YMajorTickSize => 0.5;


        protected override double GetXValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.Acceleration.XinG;
        }

        protected override double GetYValue(TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.Acceleration.ZinG;
        }
    }
}