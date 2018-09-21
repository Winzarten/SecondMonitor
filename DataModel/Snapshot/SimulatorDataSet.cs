namespace SecondMonitor.DataModel.Snapshot
{
    using BasicProperties;
    using Drivers;

    public class SimulatorDataSet
    {
        public SimulatorDataSet(string source)
        {
            Source = source;
            PedalInfo = new PedalInfo();
            SessionInfo = new SessionInfo();
            DriversInfo = new DriverInfo[30];
            
            for (int i = 0; i < DriversInfo.Length; i++)
            {
                DriversInfo[i] = new DriverInfo();
            }
            PlayerInfo = new DriverInfo();
            LeaderInfo = new DriverInfo();
        }

        public string Source { get; set; }

        public PedalInfo PedalInfo { get; set; }

        public SessionInfo SessionInfo { get; set; }

        public DriverInfo[] DriversInfo { get; set; }

        public DriverInfo PlayerInfo { get; set; }

        public DriverInfo LeaderInfo { get; set; }

        public SimulatorSourceInfo SimulatorSourceInfo { get; } = new SimulatorSourceInfo();
    }
}
