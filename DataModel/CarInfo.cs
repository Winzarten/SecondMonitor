using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel
{
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
            public WheelInfo FrontLeft;
            public WheelInfo FrontRight;
            public WheelInfo RearLeft;
            public WheelInfo RearRight;
        }

        public CarInfo()
        {
            WheelsInfo = new Wheels();
            OilSystemInfo = new OilInfo();
            FuelSystemInfo = new FuelInfo();
            WaterSystmeInfo = new WaterInfo();
            Acceleration = new Acceleration();
        }

        public Wheels WheelsInfo;
        public OilInfo OilSystemInfo;
        public FuelInfo FuelSystemInfo;
        public WaterInfo WaterSystmeInfo;
        public Acceleration Acceleration;
        
    }
}
