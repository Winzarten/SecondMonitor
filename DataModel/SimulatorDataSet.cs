using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel
{
    public class SimulatorDataSet
    {
        public SimulatorDataSet()
        {
            PlayerCarInfo = new CarInfo();
            PedalInfo = new PedalInfo();
        }
        public CarInfo PlayerCarInfo;
        public PedalInfo PedalInfo;
    }
}
