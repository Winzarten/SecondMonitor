namespace SecondMonitor.DataModel.BasicProperties
{
    public class Distance
    {
        public static Distance ZeroDistance { get; } = new Distance(0, true);


        private readonly bool _isZero;

        private Distance(double distanceInM, bool isZero = false)
        {
            DistanceInM = distanceInM;
            _isZero = isZero;
        }

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

        protected bool Equals(Distance other)
        {
            return _isZero == other._isZero && DistanceInM.Equals(other.DistanceInM);
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

        public static Distance operator -(Distance d1, Distance d2)
        {
            return FromMeters(d1.DistanceInM - d2.DistanceInM);
        }
    }
}