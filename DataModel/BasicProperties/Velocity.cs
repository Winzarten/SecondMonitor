using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Velocity
    {
        public static readonly Velocity Zero = Velocity.FromMs(0);
        private double _inMs;

        private Velocity(double ms)
        {
            _inMs = ms;
        }

        public double InKph
        {
            get => _inMs * 3.6;
        }

        public double InMs
        {
            get => _inMs;
        }

        static public Velocity FromMs(double inMs)
        {
            return new Velocity(inMs);
        }

        public static bool operator <(Velocity v1, Velocity v2)
        {
            return v1.InMs < v2.InMs;
        }
        public static bool operator >(Velocity v1, Velocity v2)
        {
            return v1.InMs > v2.InMs;
        }
        public static bool operator <=(Velocity v1, Velocity v2)
        {
            return v1.InMs <= v2.InMs;
        }
        public static bool operator >=(Velocity v1, Velocity v2)
        {
            return v1.InMs >= v2.InMs;
        }

        public static Velocity operator -(Velocity v1, Velocity v2)
        {
            return Velocity.FromMs(v1.InMs - v2.InMs);
        }
    }
}
