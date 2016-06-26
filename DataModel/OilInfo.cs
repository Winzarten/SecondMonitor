using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel
{
    public class OilInfo
    {
        public OilInfo()
        {
            OilPressure = new Pressure();
            OilTemperature = new Temperature();
        }
        public Temperature OilTemperature;
        public Pressure OilPressure;
    }
}
