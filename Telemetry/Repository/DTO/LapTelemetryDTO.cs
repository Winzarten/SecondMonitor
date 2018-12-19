namespace SecondMonitor.Telemetry.TelemetryManagement.DTO
{
    using System;
    using System.Xml.Serialization;
    using DataModel.Telemetry;

    [XmlRoot(ElementName = "LapTelemetry")]
    public class LapTelemetryDto
    {
        [XmlAttribute]
        public int LapNumber { get; set; }

        [XmlAttribute]
        public double LapTimeSeconds { get; set; }

        public TimedTelemetrySnapshot[] TimedTelemetrySnapshots { get; set; }
    }
}