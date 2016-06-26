using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Volume
    {
        private double valueInLiters;
        private Volume(double valueInLiters)
        {
            this.valueInLiters = valueInLiters;
        }
        public static Volume FromLiters(double volumeInLiters)
        {
            return new Volume(volumeInLiters);
        }

        public double InLiters
        {
            get { return valueInLiters; }
        }
    }
}
