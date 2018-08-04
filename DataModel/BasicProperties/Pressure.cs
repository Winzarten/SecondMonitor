namespace SecondMonitor.DataModel.BasicProperties
{
    using System;

    using Newtonsoft.Json;

    public class Pressure
    {
        public Pressure()
        {
            InKpa = -1;
        }

        private Pressure(double valueInKpa)
        {
            InKpa = valueInKpa;
        }

        public double InKpa { get; }

        public static string GetUnitSymbol(PressureUnits units)
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

        public static Pressure FromKiloPascals(double pressureInKpa)
        {
            return new Pressure(pressureInKpa);
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

        public string GetValueInUnits(PressureUnits units, int decimalPlaces)
        {
            switch (units)
            {
                case PressureUnits.Kpa:
                    return InKpa.ToString($"F{decimalPlaces}");
                case PressureUnits.Atmosphere:
                    return InAtmospheres.ToString($"F{decimalPlaces}");
                case PressureUnits.Bar:
                    return InBars.ToString($"F{decimalPlaces}");
                case PressureUnits.Psi:
                    return InPsi.ToString($"F{decimalPlaces}");
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        [JsonIgnore]
        public double InAtmospheres => InKpa / 101.3;

        [JsonIgnore]
        public double InBars => InKpa * 0.01;

        [JsonIgnore]
        public double InPsi => InKpa * 0.145038;
    }
}
