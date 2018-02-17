namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class CarInfo
    {
        public class Wheels
        {
            public Wheels()
            {
                this.FrontRight = new WheelInfo();
                this.FrontLeft = new WheelInfo();
                this.RearRight = new WheelInfo();
                this.RearLeft = new WheelInfo();
            }

            public WheelInfo FrontLeft { get; set; }

            public WheelInfo FrontRight { get; set; }

            public WheelInfo RearLeft { get; set; }

            public WheelInfo RearRight { get; set; }
        }

        public CarInfo()
        {
            this.WheelsInfo = new Wheels();
            this.OilSystemInfo = new OilInfo();
            this.FuelSystemInfo = new FuelInfo();
            this.WaterSystemInfo = new WaterInfo();
            this.Acceleration = new Acceleration();
        }

        public Wheels WheelsInfo { get; set; }

        public OilInfo OilSystemInfo { get; set; }

        public FuelInfo FuelSystemInfo { get; set; }

        public WaterInfo WaterSystemInfo { get; set; }

        public Acceleration Acceleration { get; set; }

        public string CurrentGear { get; set; } = string.Empty;

    }
}
