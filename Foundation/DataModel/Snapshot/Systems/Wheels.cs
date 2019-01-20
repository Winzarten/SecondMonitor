namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

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

        public WheelInfo FrontLeft { get; set; }

        public WheelInfo FrontRight { get; set; }

        public WheelInfo RearLeft { get; set; }

        public WheelInfo RearRight { get; set; }
    }
}