using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel
{
    public class Pressure
    {
        double valueInKpa;        

        private Pressure(double valueInKpa)
        {
            this.valueInKpa = valueInKpa;
        }

        public double InKpa
        {
            get { return valueInKpa; }

        }

        public static Pressure FromKiloPascals(double pressureInKpa)
        {
            return new Pressure(pressureInKpa);
        }
    }
}
