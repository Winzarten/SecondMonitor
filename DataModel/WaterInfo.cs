using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel
{
    public class WaterInfo
    {
        public WaterInfo()
        {
            WaterTemperature = new Temperature();
        }
        public Temperature WaterTemperature;
    }
}
