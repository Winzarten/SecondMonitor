namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.ScatterPlot.Extractors
{
    using System.Collections.Generic;
    using DataModel.Snapshot.Systems;
    using DataModel.Telemetry;
    using Filter;
    using SecondMonitor.ViewModels.Settings;
    using Settings;

    public class LateralAccelerationToRideHeightExtractor : HorizontalAccelerationToRideHeightExtractor
    {
        public LateralAccelerationToRideHeightExtractor(ISettingsProvider settingsProvider, IEnumerable<ITelemetryFilter> filters) : base(settingsProvider, filters)
        {
        }

        protected override double GetXWheelValue(WheelInfo wheelInfo, TimedTelemetrySnapshot snapshot)
        {
            return snapshot.PlayerData.CarInfo.Acceleration.XinG;
        }

    }
}