using System;

namespace SecondMonitor.DataModel
{
    public class Pressure
    {
        public enum PressureUnits { Kpa, Atmosphere }
        private double valueInKpa;      
        
        public Pressure()
        {
            valueInKpa = -1;
        }

        private Pressure(double valueInKpa)
        {
            this.valueInKpa = valueInKpa;
        }

        public double InKpa
        {
            get { return valueInKpa; }

        }

        public double GetValueInUnits(PressureUnits units)
        {
            switch (units)
            {
                case PressureUnits.Kpa:
                    return InKpa;
                case PressureUnits.Atmosphere:
                    return InAtmospheres;               
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        static public string GetUnitSymbol(PressureUnits units)
        {
            switch (units)
            {
                case PressureUnits.Kpa:
                    return " KPa";
                case PressureUnits.Atmosphere:
                    return " ATM";                
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
        }
        public double InAtmospheres
        {
            get { return InKpa / 101.3; }
        }

        public static Pressure FromKiloPascals(double pressureInKpa)
        {
            return new Pressure(pressureInKpa);
        }
    }
}
