namespace SecondMonitor.DataModel.BasicProperties
{
    public class PedalInfo
    {
        public PedalInfo()
        {
            BrakePedalPosition = -1;
            ThrottlePedalPosition = -1;
            ClutchPedalPosition = -1;
        }
        public double BrakePedalPosition;
        public double ThrottlePedalPosition;
        public double ClutchPedalPosition;
    }
}
