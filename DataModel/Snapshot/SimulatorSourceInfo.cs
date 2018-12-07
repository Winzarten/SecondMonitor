namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    [Serializable]
    public class SimulatorSourceInfo
    {
        public SimulatorSourceInfo()
        {
            TelemetryInfo = new TelemetryInfo();
        }

        public bool HasLapTimeInformation { get; set; }

        public DataInputSupport SectorTimingSupport { get; set; } = DataInputSupport.None;

        // Some sims, like rFactor do not show a clear change in laps/lap status when the driver crosses the finish line in his out lap and moves to the hot lap. app needs to use alternative methods to detect this state
        public bool SimNotReportingEndOfOutLapCorrectly { get; set; }

        // Some sims, like r3e automatically complete the final lap after the playre crosses the line
        public bool AIInstantFinish { get; set; }

        // Some sims, like AC do not report out lap as invalid
        public bool OutLapIsValid { get; set; }

        // For some sims (i.e. AMS) it is difficult tu check if lap is valid. One of the methods to look if the sectors were updated. This flag allows this behavior.
        public bool InvalidateLapBySector { get; set; }

        // Some sims do not update all relevant information when lap changes. This flag forces a pending state when lap changes, allowing all relevant information to be updated properly.
        public bool ForceLapOverTime { get; set; }

        // Indicates if the sim has a global pool of tyre compounds (i.e. assetto corsa), or if each car has its own. The latter means that tyre compound for two cars can have different properties, even if the name is the same
        public bool GlobalTyreCompounds { get; set; }

        //Indicates that the world positions (x,y,z) provided by the sim are not valid.
        public bool WorldPositionInvalid { get; set; }

        public TelemetryInfo TelemetryInfo { get; set; }
    }
}