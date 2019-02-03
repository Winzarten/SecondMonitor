namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using System.Xml.Serialization;
    using Drivers;

    [Serializable]
    public sealed class SimulatorSourceInfo
    {
        public SimulatorSourceInfo()
        {
            TelemetryInfo = new TelemetryInfo();
        }

        [XmlAttribute]
        public bool HasLapTimeInformation { get; set; }

        [XmlAttribute]
        public DataInputSupport SectorTimingSupport { get; set; } = DataInputSupport.None;

        [XmlAttribute]
        // Some sims, like rFactor do not show a clear change in laps/lap status when the driver crosses the finish line in his out lap and moves to the hot lap. app needs to use alternative methods to detect this state
        public bool SimNotReportingEndOfOutLapCorrectly { get; set; }

        [XmlAttribute]
        // Some sims, like r3e automatically complete the final lap after the playre crosses the line
        public bool AIInstantFinish { get; set; }

        [XmlAttribute]
        // Some sims, like AC do not report out lap as invalid
        public bool OutLapIsValid { get; set; }

        [XmlAttribute]
        // For some sims (i.e. AMS) it is difficult tu check if lap is valid. One of the methods to look if the sectors were updated. This flag allows this behavior.
        public bool InvalidateLapBySector { get; set; }

        [XmlAttribute]
        // Some sims do not update all relevant information when lap changes. This flag forces a pending state when lap changes, allowing all relevant information to be updated properly.
        public bool ForceLapOverTime { get; set; }

        [XmlAttribute]
        // Indicates if the sim has a global pool of tyre compounds (i.e. assetto corsa), or if each car has its own. The latter means that tyre compound for two cars can have different properties, even if the name is the same
        public bool GlobalTyreCompounds { get; set; }

        [XmlAttribute]
        //Indicates that the world positions (x,y,z) provided by the sim are not valid.
        public bool WorldPositionInvalid { get; set; }

        [XmlAttribute]
        //Indicates if the time is interpolated - i.e RF2 only refresh time every 200ms, between those 200ms the connector will interpolate the time
        public bool TimeInterpolated { get; set; }

        [XmlAttribute]
        public GapInformationKind GapInformationProvided { get; set; }

        public TelemetryInfo TelemetryInfo { get; set; }
    }
}