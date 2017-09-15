using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.Drivers;

namespace SecondMonitor.DataModel
{
    public class SimulatorDataSet
    {
        public SimulatorDataSet()
        {
            PlayerCarInfo = new CarInfo();
            PedalInfo = new PedalInfo();
            SessionInfo = new SessionInfo();
            DriversInfo = new DriverInfo[0];
        }
        public CarInfo PlayerCarInfo;
        public PedalInfo PedalInfo;
        public SessionInfo SessionInfo;
        public DriverInfo[] DriversInfo;
    }
}
