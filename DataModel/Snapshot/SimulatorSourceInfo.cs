namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    [Serializable]
    public class SimulatorSourceInfo
    {
        public SimulatorSourceInfo()
        {

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

        public bool ForceLapOverTime { get; set; }
    }
}