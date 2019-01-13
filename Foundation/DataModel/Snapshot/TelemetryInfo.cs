namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class TelemetryInfo
    {
        [XmlAttribute]
        public bool RequiresDistanceInterpolation { get; set; }

        [XmlAttribute]
        public bool RequiresPositionInterpolation { get; set; }
    }
}