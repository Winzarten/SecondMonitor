namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public sealed class Wheels
    {
        public Wheels()
        {
            FrontRight = new WheelInfo();
            FrontLeft = new WheelInfo();
            RearRight = new WheelInfo();
            RearLeft = new WheelInfo();
        }

        [XmlIgnore]
        public WheelInfo[] AllWheels => new WheelInfo[] { FrontLeft, FrontRight, RearLeft, RearRight };

        public WheelInfo FrontLeft { get; set; }

        public WheelInfo FrontRight { get; set; }

        public WheelInfo RearLeft { get; set; }

        public WheelInfo RearRight { get; set; }
    }
}