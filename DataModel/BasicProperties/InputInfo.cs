namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    [Serializable]
    public class InputInfo
    {
        public InputInfo()
        {
            BrakePedalPosition = -1;
            ThrottlePedalPosition = -1;
            ClutchPedalPosition = -1;
        }

        public double BrakePedalPosition { get; set; }

        public double ThrottlePedalPosition { get; set; }

        public double ClutchPedalPosition { get; set; }

        public double SteeringInput { get; set; }

        public double WheelAngle { get; set; }
    }
}
