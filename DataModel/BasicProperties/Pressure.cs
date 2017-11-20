using System;

namespace SecondMonitor.DataModel
{
    public class Pressure
    {
        private double _valueInKpa;      
        
        public Pressure()
        {
            _valueInKpa = -1;
        }

        private Pressure(double valueInKpa)
        {
            this._valueInKpa = valueInKpa;
        }

        public double InKpa
        {
            get { return _valueInKpa; }

        }

        public double GetValueInUnits(PressureUnits units)
        {
            switch (units)
            {
                case PressureUnits.Kpa:
                    return InKpa;
                case PressureUnits.Atmosphere:
                    return InAtmospheres;
                case PressureUnits.Bar:
                    return InBars;
                case PressureUnits.Psi:
                    return InPsi;
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
                case PressureUnits.Bar:
                    return "Bar";
                case PressureUnits.Psi:
                    return "Psi";
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
        }
        public double InAtmospheres
        {
            get { return InKpa / 101.3; }
        }

        public double InBars
        {
            get { return InKpa * 0.01; }
        }

        public double InPsi
        {
            get { return InKpa * 0.145038; }
        }

        public static Pressure FromKiloPascals(double pressureInKpa)
        {
            return new Pressure(pressureInKpa);
        }
    }
}
