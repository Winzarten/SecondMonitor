namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts.Filter
{
    using DataModel.Telemetry;

    public class GearTelemetryFilter : IGearTelemetryFilter
    {

        public string FilterGear { get; set; }

        public bool Accepts(TimedTelemetrySnapshot dataSet)
        {
            return string.IsNullOrWhiteSpace(FilterGear) || dataSet.PlayerData.CarInfo.CurrentGear == FilterGear;
        }


    }
}