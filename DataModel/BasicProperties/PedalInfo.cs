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

        public double BrakePedalPosition { get; set; }

        public double ThrottlePedalPosition { get; set; }

        public double ClutchPedalPosition { get; set; }
    }
}
