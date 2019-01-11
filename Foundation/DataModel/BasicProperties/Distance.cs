namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class Distance : IQuantity
    {

        private readonly bool _isZero;

        public Distance()
        {

        }

        private Distance(double inMeters, bool isZero = false)
        {
            InMeters = inMeters;
            _isZero = isZero;
        }

        [XmlIgnore]
        public static Distance ZeroDistance { get; } = new Distance(0, true);

        [XmlAttribute]
        public double InMeters { get; set; }

        [XmlIgnore]
        public double InKilometers => InMeters / 1000;

        [XmlIgnore]
        public double InMiles => InKilometers / 1.609344;

        [XmlIgnore]
        public double InInches => InMeters * 39.3701;

        [XmlIgnore]
        public double InCentimeters => InMeters * 100;

        [XmlIgnore]
        public double InYards => InMeters * 1.09361;

        [XmlIgnore]
        public double InFeet => InMeters * 3.28084;

        public double GetByUnit(DistanceUnits distanceUnits)
        {
            switch (distanceUnits)
            {
                case DistanceUnits.Meters:
                    return InMeters;
                case DistanceUnits.Kilometers:
                    return InKilometers;
                case DistanceUnits.Miles:
                    return InMiles;
                case DistanceUnits.Feet:
                    return InFeet;
                case DistanceUnits.Inches:
                    return InInches;
                case DistanceUnits.Centimeter:
                    return InCentimeters;
                case DistanceUnits.Yards:
                    return InYards;
                default:
                    throw new ArgumentException($"Distance units {distanceUnits} is not known");
            }
        }

        public static Distance FromMeters(double distanceInM)
        {
            return new Distance(distanceInM);
        }

        public static bool operator ==(Distance dist1, Distance dist2)
        {
            return dist1?._isZero == dist2?._isZero && dist1?.InMeters == dist2?.InMeters;
        }

        public static bool operator !=(Distance dist1, Distance dist2)
        {
            return !(dist1 == dist2);
        }

        public static Distance operator -(Distance d1, Distance d2)
        {
            return FromMeters(d1.InMeters - d2.InMeters);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;

            }

            if (ReferenceEquals(this, obj))
            {
                return true;

            }

            return obj.GetType() == GetType() && Equals((Distance)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_isZero.GetHashCode() * 397) ^ InMeters.GetHashCode();
            }
        }

        protected bool Equals(Distance other)
        {
            return _isZero == other._isZero && InMeters.Equals(other.InMeters);
        }

        public IQuantity ZeroQuantity => ZeroDistance;
        public bool IsZero => InMeters == 0;
        public double RawValue => InMeters;

        public static string GetUnitsSymbol(DistanceUnits distanceUnits)
        {
            switch (distanceUnits)
            {
                case DistanceUnits.Meters:
                    return "m";
                case DistanceUnits.Kilometers:
                    return "Km";
                case DistanceUnits.Miles:
                    return "mi";
                case DistanceUnits.Feet:
                    return "ft";
                case DistanceUnits.Inches:
                    return "in";
                case DistanceUnits.Centimeter:
                    return "cm";
                case DistanceUnits.Yards:
                    return "yd";
                default:
                    throw new ArgumentException($"Distance units {nameof(distanceUnits)} is unknown.");
            }
        }

        public static Distance CreateByUnits(double value, DistanceUnits distanceUnits)
        {
            switch (distanceUnits)
            {
                case DistanceUnits.Meters:
                    return FromMeters(value);
                case DistanceUnits.Kilometers:
                    return FromMeters(value * 1000);
                case DistanceUnits.Miles:
                    return FromMeters(value * 1609.34);
                case DistanceUnits.Feet:
                    return FromMeters(value * 0.3047992424196);
                case DistanceUnits.Yards:
                    return FromMeters(value * 0.9144);
                case DistanceUnits.Inches:
                    return FromMeters(value * 0.0254);
                case DistanceUnits.Centimeter:
                    return FromMeters(value / 100);
                default:
                    throw new ArgumentException($"Distance units {nameof(distanceUnits)} is unknown.");
            }
        }

        public static Distance operator *(Distance d1, double d)
        {
            if(d1 is null)
            {
                return null;
            }

            return FromMeters(d1.InMeters * d);
        }

        public static Distance operator *(Distance d1, Distance d2)
        {
            if (d1 is null || d2 is null)
            {
                return null;
            }

            return FromMeters(d1.InMeters * d2.InMeters);
        }

        public static bool operator >(Distance d1, Distance d2)
        {
            if (d1 is null || d2 is null)
            {
                return false;
            }

            return d1.InMeters > d2.InMeters;
        }

        public static bool operator <(Distance d1, Distance d2)
        {
            if (d1 is null || d2 is null)
            {
                return false;
            }

            return d1.InMeters < d2.InMeters;
        }
    }
}