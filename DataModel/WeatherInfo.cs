using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel
{
    public class WeatherInfo
    {
        public Temperature airTemperature = Temperature.FromCelsius(-1);
        public Temperature trackTemperature = Temperature.FromCelsius(-1);
        public int rainIntensity = 0;
    }
}
