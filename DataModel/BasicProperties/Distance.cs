namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class Distance
    {

        private readonly bool _isZero;

        public Distance()
        {

        }

        private Distance(double distanceInM, bool isZero = false)
        {
            DistanceInM = distanceInM;
            _isZero = isZero;
        }

        [XmlIgnore]
        public static Distance ZeroDistance { get; } = new Distance(0, true);

        public double DistanceInM { get; }

        public static Distance FromMeters(double distanceInM)
        {
            return new Distance(distanceInM);
        }

        public static bool operator ==(Distance dist1, Distance dist2)
        {
            return dist1?._isZero == dist2?._isZero && dist1?.DistanceInM == dist2?.DistanceInM;
        }

        public static bool operator !=(Distance dist1, Distance dist2)
        {
            return !(dist1 == dist2);
        }

        public static Distance operator -(Distance d1, Distance d2)
        {
            return FromMeters(d1.DistanceInM - d2.DistanceInM);
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

            return obj.GetType() == this.GetType() && Equals((Distance)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_isZero.GetHashCode() * 397) ^ DistanceInM.GetHashCode();
            }
        }

        protected bool Equals(Distance other)
        {
            return _isZero == other._isZero && DistanceInM.Equals(other.DistanceInM);
        }
    }
}