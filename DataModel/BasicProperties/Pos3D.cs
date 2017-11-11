using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Point3D
    {
        public Point3D( double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Double X { get; private set; } = 0;
        public Double Y { get; private set; } = 0;
        public Double Z { get; private set; } = 0;
    }
}
