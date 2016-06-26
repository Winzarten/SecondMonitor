using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel
{
    public class CarInfo
    {
        public class Wheels
        {
            public WheelInfo FrontLeft;
            public WheelInfo FrontRight;
            public WheelInfo RearLeft;
            public WheelInfo RearRight;
        }

        public Wheels WheelsInfo;
        public OilInfo OilSystemInfo;
        public FuelInfo FuelSystemInfo;
        public WaterInfo WaterSystmeInfo;
        
    }
}
