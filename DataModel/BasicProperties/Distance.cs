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

        public double InMeters { get; }

        [XmlIgnore]
        public double InKilometers => InMeters / 1000;

        [XmlIgnore]
        public double InMiles => InKilometers / 1.609344;

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(distanceUnits), distanceUnits, null);
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