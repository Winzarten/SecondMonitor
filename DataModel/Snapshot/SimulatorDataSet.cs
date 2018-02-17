namespace SecondMonitor.DataModel.Snapshot
{
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    public class SimulatorDataSet
    {        
        public SimulatorDataSet(string source)
        {
            this.Source = source;
            this.PedalInfo = new PedalInfo();
            this.SessionInfo = new SessionInfo();
            this.DriversInfo = new DriverInfo[0];
            this.PlayerInfo = new DriverInfo();
            this.LeaderInfo = new DriverInfo();
        }

        public string Source { get; set; }

        public PedalInfo PedalInfo { get; set; }

        public SessionInfo SessionInfo { get; set; }

        public DriverInfo[] DriversInfo { get; set; }

        public DriverInfo PlayerInfo { get; set; }

        public DriverInfo LeaderInfo { get; set; }

        public SimulatorSourceInfo  SimulatorSourceInfo { get; } = new SimulatorSourceInfo();
    }
}
