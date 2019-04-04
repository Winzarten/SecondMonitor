using System;
using Newtonsoft.Json;

namespace SecondMonitor.DataModel.BasicProperties
{
    [Serializable]
    public class Force : IQuantity
    {
        public Force()
        {
            IsZero = true;
            InNewtons = 0;
        }

        private Force(double inNewtons)
        {
            IsZero = false;
            InNewtons = inNewtons;
        }

        public double InNewtons { get; set; }

        [JsonIgnore]
        public double InPoundForce => 0.224809 * InNewtons;

        public Force Zero => new Force();
        public IQuantity ZeroQuantity => Zero;
        public bool IsZero { get; }

        public double RawValue => InNewtons;

        public Force FromNewtons(double force)
        {
            return new Force(force);
        }

        public double GetValueInUnits(ForceUnits units)
        {
            switch (units)
            {
                case ForceUnits.Newtons:
                    return InNewtons;
                case ForceUnits.PoundForce:
                    return InPoundForce;
                default:
                    throw new ArgumentOutOfRangeException(nameof(units), units, null);
            }
        }

        public static string GetUnitSymbol(ForceUnits units)
        {
            switch (units)
            {
                case ForceUnits.Newtons:
                    return "N";
                case ForceUnits.PoundForce:
                    return "lbf";
                default:
                    throw new ArgumentOutOfRangeException(nameof(units), units, null);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (!(obj is Force force))
            {
                return false;
            }

            return InNewtons == force.InNewtons;
        }

        protected bool Equals(Force other)
        {
            return InNewtons.Equals(other.InNewtons) && IsZero == other.IsZero;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (InNewtons.GetHashCode() * 397) ^ IsZero.GetHashCode();
            }
        }
    }
}