using System;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public double InMs
        {
            get => _inMs;
        }

        [JsonIgnore]
        public double InMph
        {
            get => _inMs * 2.23694;
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

        public double GetValueInUnits(VelocityUnits units)
        {
            switch (units)
            {
                case VelocityUnits.Kph:
                    return InKph;
                case VelocityUnits.Mph:
                    return InMph;
                case VelocityUnits.Ms:
                    return InMs;
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        public static string GetUnitSymbol(VelocityUnits units)
        {
            switch (units)
            {
                case VelocityUnits.Kph:
                    return "Kph";
                case VelocityUnits.Mph:
                    return "Mph";
                case VelocityUnits.Ms:
                    return "Ms";
            }
            throw new ArgumentException("Unable to return symbol for" + units.ToString());
        }

        public static Velocity operator -(Velocity v1, Velocity v2)
        {
            return Velocity.FromMs(v1.InMs - v2.InMs);
        }
    }
}
