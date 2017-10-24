using System;

namespace SecondMonitor.DataModel.BasicProperties
{
    public class Volume
    {
        private double valueInLiters;

        public Volume()
        {
            valueInLiters = -1;
        }

        private Volume(double valueInLiters)
        {
            this.valueInLiters = valueInLiters;
        }
        public static Volume FromLiters(double volumeInLiters)
        {
            return new Volume(volumeInLiters);
        }

        public double GetValueInUnits(VolumeUnits units)
        {
            switch (units)
            {
                case VolumeUnits.Liters:
                    return InLiters;
                case VolumeUnits.US_Gallons:
                    return InUSGallons;
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        static public string GetUnitSymbol(VolumeUnits units)
        {
            switch (units)
            {
                case VolumeUnits.Liters:
                    return "L";
                case VolumeUnits.US_Gallons:
                    return "gal";
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
        }

        public override bool Equals(object obj)
        {
            var volume = obj as Volume;
            return volume != null &&
                   InLiters == volume.InLiters;                   
        }

        public override int GetHashCode()
        {
            var hashCode = 1337393187;
            hashCode = hashCode * -1521134295 + valueInLiters.GetHashCode();
            hashCode = hashCode * -1521134295 + InLiters.GetHashCode();
            hashCode = hashCode * -1521134295 + InUSGallons.GetHashCode();
            return hashCode;
        }

        public double InLiters
        {
            get { return valueInLiters; }
        }

        public double InUSGallons
        {
            get { return valueInLiters * 0.264172; }
        }

        public static Volume operator +(Volume left, Volume right)
        {
            return Volume.FromLiters(left.InLiters + right.InLiters);
        }
        public static Volume operator -(Volume left, Volume right)
        {
            return Volume.FromLiters(left.InLiters - right.InLiters);
        }
        public static Volume operator *(Volume left, Volume right)
        {
            return Volume.FromLiters(left.InLiters * right.InLiters);
        }
        public static Volume operator *(Volume left, double right)
        {
            return Volume.FromLiters(left.InLiters * right);
        }
        public static Volume operator *(Volume left, int right)
        {
            return Volume.FromLiters(left.InLiters * right);
        }
        public static Volume operator *(int left, Volume right)
        {
            return Volume.FromLiters(left * right.InLiters);
        }
        public static Volume operator /(Volume left, Volume right)
        {
            return Volume.FromLiters(left.InLiters / right.InLiters);
        }
        public static Volume operator /(Volume left, double right)
        {
            return Volume.FromLiters(left.InLiters / right);
        }
        public static bool operator ==(Volume left, Volume right)
        {
            return left.InLiters == right.InLiters;
        }
        public static bool operator !=(Volume left, Volume right)
        {
            return left.InLiters != right.InLiters;
        }

        public static bool operator <(Volume left, Volume right)
        {
            return left.InLiters < right.InLiters;
        }
        public static bool operator >(Volume left, Volume right)
        {
            return left.InLiters > right.InLiters;
        }

        public static bool operator <=(Volume left, Volume right)
        {
            return left.InLiters <= right.InLiters;
        }
        public static bool operator >=(Volume left, Volume right)
        {
            return left.InLiters >= right.InLiters;
        }


    }
}
