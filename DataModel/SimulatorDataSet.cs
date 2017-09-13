using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel
{
    public class SimulatorDataSet
    {
        public SimulatorDataSet()
        {
            PlayerCarInfo = new CarInfo();
            PedalInfo = new PedalInfo();
            SessionInfo = new SessionInfo();
        }
        public CarInfo PlayerCarInfo;
        public PedalInfo PedalInfo;
        public SessionInfo SessionInfo;
    }
}
