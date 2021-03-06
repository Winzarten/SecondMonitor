﻿namespace SecondMonitor.DataModel.Snapshot
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

        [XmlAttribute]
        public bool ContainsSuspensionVelocity { get; set; }

        [XmlAttribute]
        public bool ContainsSuspensionTravel { get; set; }

        [XmlAttribute]
        public bool ContainsOptimalTemperatures { get; set; }
    }
}