namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public sealed class TelemetryInfo
    {
        [XmlAttribute]
        public bool RequiresDistanceInterpolation { get; set; }

        [XmlAttribute]
        public bool RequiresPositionInterpolation { get; set; }

        public bool ContainsSuspensionVelocity { get; set; }

        public bool ContainsSuspensionTravel { get; set; }
    }
}