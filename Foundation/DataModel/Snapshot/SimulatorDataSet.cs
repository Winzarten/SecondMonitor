namespace SecondMonitor.DataModel.Snapshot
{
    using System;

    using BasicProperties;
    using Drivers;

    [Serializable]
    public sealed class SimulatorDataSet
    {
        public SimulatorDataSet(string source)
        {
            Source = source;
            InputInfo = new InputInfo();
            SessionInfo = new SessionInfo();
            DriversInfo = new DriverInfo[0];
            PlayerInfo = new DriverInfo();
            LeaderInfo = new DriverInfo();
        }

        public string Source { get; set; }

        public InputInfo InputInfo { get; set; }

        public SessionInfo SessionInfo { get; set; }

        public DriverInfo[] DriversInfo { get; set; }

        public DriverInfo PlayerInfo { get; set; }

        public DriverInfo LeaderInfo { get; set; }

        public SimulatorSourceInfo SimulatorSourceInfo { get; } = new SimulatorSourceInfo();
    }
}
