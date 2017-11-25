using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Acceleration
    {

        private double _x;
        private double _y;
        private double _z;

        private static readonly double GConst = 9.8;

        public Acceleration()
        {

        }
        [JsonIgnore]
        public double XinG
        {
            get { return _x / GConst; }
            set { _x = value * GConst; }
        }
        [JsonIgnore]
        public double YinG
        {
            get { return _y / GConst; }
            set { _y = value * GConst; }
        }
        [JsonIgnore]
        public double ZinG
        {
            get { return _z / GConst; }
            set { _z = value * GConst; }
        }

        public double XinMs
        {
            get { return _x;  }
            set { _x = value; }
        }

        public double YinMs
        {
            get { return _y; }
            set { _y = value; }
        }

        public double ZinMs
        {
            get { return _z; }
            set { _z = value; }
        }


    }
}
