namespace SecondMonitor.DataModel
{
    public class SimulatorSourceInfo
    {
        public bool HasLapTimeInformation { get; set; }

        public DataInputSupport SectorTimingSupport { get; set; } = DataInputSupport.NONE;
    }
}