using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel
{
    public class FuelInfo
    {
        public FuelInfo()
        {
            FuelCapacity = new Volume();
            FuelRemaining = new Volume();
            FuelPressure = new Pressure();
        }
        public Volume FuelCapacity;
        public Volume FuelRemaining;
        public Pressure FuelPressure;
    }
}
