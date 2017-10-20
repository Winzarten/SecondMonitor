using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.DataModel
{
    public class SimulatorDataSet
    {
        public string Source = "None";
        public SimulatorDataSet(string source)
        {
            this.Source = source;
            PedalInfo = new PedalInfo();
            SessionInfo = new SessionInfo();
            DriversInfo = new DriverInfo[0];
            PlayerInfo = new DriverInfo();
            LeaderInfo = new DriverInfo();
        }
        public PedalInfo PedalInfo;
        public SessionInfo SessionInfo;
        public DriverInfo[] DriversInfo;
        public DriverInfo PlayerInfo;
        public DriverInfo LeaderInfo;
    }
}
