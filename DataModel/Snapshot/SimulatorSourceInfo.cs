namespace SecondMonitor.DataModel.Snapshot
{
    public class SimulatorSourceInfo
    {
        public bool HasLapTimeInformation { get; set; }

        public DataInputSupport SectorTimingSupport { get; set; } = DataInputSupport.NONE;

        //Some sims, like rFactor do not show a clear change in laps/lap status when the driver crosses the finish line in his out lap and moves to the hot lap. app needs to use alternative methods to detect this state
        public bool SimNotReportingEndOfOutLapCorrectly { get; set; }

        //Some sims, like r3e automatically complete the final lap after the playre crosses the line
        public bool AIInstantFinish { get; set; }
    }
}