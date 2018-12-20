namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System.Xml.Serialization;
    using DataModel.Telemetry;

    [XmlRoot(ElementName = "LapTelemetry")]
    public class LapTelemetryDto
    {
        public LapSummaryDto LapSummary { get; set; }

        public TimedTelemetrySnapshot[] TimedTelemetrySnapshots { get; set; }
    }
}