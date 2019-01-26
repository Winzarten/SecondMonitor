namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System;
    using System.Xml.Serialization;
    using DataModel.Telemetry;

    [XmlRoot(ElementName = "LapTelemetry")]
    [Serializable]
    public class LapTelemetryDto
    {
        public LapSummaryDto LapSummary { get; set; }

        public TimedTelemetrySnapshot[] TimedTelemetrySnapshots { get; set; }
    }
}