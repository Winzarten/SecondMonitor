namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public class Volume : IQuantity
    {
        public Volume()
        {
            InLiters = -1;
        }

        private Volume(double valueInLiters)
        {
            InLiters = valueInLiters;
        }

        public double InLiters { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public double InUsGallons => InLiters * 0.264172;

        [JsonIgnore]
        [XmlIgnore]
        public IQuantity ZeroQuantity => new Volume();

        [JsonIgnore]
        [XmlIgnore]
        public bool IsZero => InLiters == -1;

        [JsonIgnore]
        [XmlIgnore]
        public double RawValue => InLiters;

        #region Operators

        public static Volume operator +(Volume left, Volume right)
        {
            return FromLiters(left.InLiters + right.InLiters);
        }

        public static Volume operator -(Volume left, Volume right)
        {
            return FromLiters(left.InLiters - right.InLiters);
        }

        public static Volume operator *(Volume left, Volume right)
        {
            return FromLiters(left.InLiters * right.InLiters);
        }

        public static Volume operator *(Volume left, double right)
        {
            return FromLiters(left.InLiters * right);
        }

        public static Volume operator *(Volume left, int right)
        {
            return FromLiters(left.InLiters * right);
        }

        public static Volume operator *(int left, Volume right)
        {
            return FromLiters(left * right.InLiters);
        }

        public static Volume operator /(Volume left, Volume right)
        {
            return FromLiters(left.InLiters / right.InLiters);
        }

        public static Volume operator /(Volume left, double right)
        {
            return FromLiters(left.InLiters / right);
        }

        public static bool operator ==(Volume left, Volume right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null))
            {
                return false;
            }

            if (ReferenceEquals(right, null))
            {
                return false;

            }
            return left.InLiters == right.InLiters;
        }

        public static bool operator !=(Volume left, Volume right)
        {
           return !(left == right);
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

        #endregion

        public static string GetUnitSymbol(VolumeUnits units)
        {
            switch (units)
            {
                case VolumeUnits.Liters:
                    return "L";
                case VolumeUnits.UsGallons:
                    return "gal";
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
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
                case VolumeUnits.UsGallons:
                    return InUsGallons;
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        public string GetValueInUnits(VolumeUnits units, int decimalPlaces)
        {
            switch (units)
            {
                case VolumeUnits.Liters:
                    return InLiters.ToString($"F{decimalPlaces}");
                case VolumeUnits.UsGallons:
                    return InUsGallons.ToString($"F{decimalPlaces}");
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        public override bool Equals(object obj)
        {
            var volume = obj as Volume;
            return volume != null && InLiters == volume.InLiters;
        }

        public override int GetHashCode()
        {
            var hashCode = 1337393187;
            hashCode = hashCode * (-1521134295 + InLiters.GetHashCode());
            hashCode = hashCode * (-1521134295 + InLiters.GetHashCode());
            hashCode = hashCode * (-1521134295 + InUsGallons.GetHashCode());
            return hashCode;
        }





    }
}
