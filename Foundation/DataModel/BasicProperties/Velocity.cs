namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public sealed class Velocity : IQuantity
    {
        public static readonly Velocity Zero = FromMs(0);

        public Velocity()
        {

        }

        private Velocity(double ms)
        {
            InMs = ms;
        }

        [XmlAttribute]
        public double InKph
        {
            get => InMs * 3.6;
            set => InMs = value / 3.6;
        }

        [JsonIgnore]
        [XmlIgnore]
        public double InMs { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public double InMph => InMs * 2.23694;

        [JsonIgnore]
        [XmlIgnore]
        public IQuantity ZeroQuantity => Zero;

        [JsonIgnore]
        [XmlIgnore]
        public bool IsZero => this == Zero;

        [JsonIgnore]
        [XmlIgnore]
        public double RawValue => InMs;

        [JsonIgnore]
        [XmlIgnore]
        public double InFps => InMs * 3.28084;

        [JsonIgnore]
        [XmlIgnore]
        public double InInPerSecond => InMs * 39.3701;

        [JsonIgnore]
        [XmlIgnore]
        public double InCentimeterPerSecond => InMs * 100;

        [JsonIgnore]
        [XmlIgnore]
        public double InMillimeterPerSecond => InMs * 1000;

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
                case VelocityUnits.Fps:
                    return "fps";
                case VelocityUnits.CmPerSecond:
                    return "cm/s";
                case VelocityUnits.InPerSecond:
                    return "In/s";
                case VelocityUnits.MMPerSecond:
                    return "mm/s";
                default:
                    throw new ArgumentOutOfRangeException(nameof(units), units, null);
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
                case VelocityUnits.Fps:
                    return InFps;
                case VelocityUnits.CmPerSecond:
                    return InCentimeterPerSecond;
                case VelocityUnits.InPerSecond:
                    return InInPerSecond;
                case VelocityUnits.MMPerSecond:
                    return InMillimeterPerSecond;
                default:
                    throw new ArgumentException("Unable to return value in" + units);
            }
        }

        public string GetValueInUnits(VelocityUnits units, int decimalPlaces)
        {
            return GetValueInUnits(units).ToString($"F{decimalPlaces}");
        }
    }
}
