namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class CarInfo
    {
        public class Wheels
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

        public CarInfo()
        {
            WheelsInfo = new Wheels();
            OilSystemInfo = new OilInfo();
            FuelSystemInfo = new FuelInfo();
            WaterSystemInfo = new WaterInfo();
            Acceleration = new Acceleration();
        }

        public Wheels WheelsInfo { get; set; }

        public OilInfo OilSystemInfo { get; set; }

        public FuelInfo FuelSystemInfo { get; set; }

        public WaterInfo WaterSystemInfo { get; set; }

        public Acceleration Acceleration { get; set; }

        public string CurrentGear { get; set; } = string.Empty;

    }
}
