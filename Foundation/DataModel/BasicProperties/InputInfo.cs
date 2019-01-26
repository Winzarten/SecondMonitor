namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public sealed class InputInfo
    {
        public InputInfo()
        {
            BrakePedalPosition = -1;
            ThrottlePedalPosition = -1;
            ClutchPedalPosition = -1;
        }

        [XmlAttribute]
        public double BrakePedalPosition { get; set; }

        [XmlAttribute]
        public double ThrottlePedalPosition { get; set; }

        [XmlAttribute]
        public double ClutchPedalPosition { get; set; }

        [XmlAttribute]
        public double SteeringInput { get; set; }

        [XmlAttribute]
        public double WheelAngle { get; set; }
    }
}
