using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Acceleration
    {

        private double x;
        private double y;
        private double z;

        private static readonly double gConst = 9.8;

        public Acceleration()
        {

        }

        public double XInG
        {
            get { return x / gConst; }
            set { x = value * gConst; }
        }

        public double YInG
        {
            get { return y / gConst; }
            set { y = value * gConst; }
        }

        public double ZInG
        {
            get { return z / gConst; }
            set { z = value * gConst; }
        }

        public double XInMS
        {
            get { return x;  }
            set { x = value; }
        }

        public double YInMS
        {
            get { return y; }
            set { y = value; }
        }

        public double ZInMS
        {
            get { return z; }
            set { z = value; }
        }


    }
}
