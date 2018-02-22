namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    using Newtonsoft.Json;

    public class Velocity
    {
        public static readonly Velocity Zero = FromMs(0);

        private Velocity(double ms)
        {
            InMs = ms;
        }

        public double InKph => InMs * 3.6;

        [JsonIgnore]
        public double InMs { get; }

        [JsonIgnore]
        public double InMph => InMs * 2.23694;

        public static Velocity FromMs(double inMs)
        {
            return new Velocity(inMs);
        }

        public static Velocity FromKph(double inKph)
        {
            return new Velocity(inKph / 3.6);
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
            return FromMs(v1.InMs - v2.InMs);
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
                default:
                    throw new ArgumentException("Unable to return value in" + units.ToString());
            }
        }
    }
}
