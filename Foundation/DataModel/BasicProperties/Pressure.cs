namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public sealed class Pressure : IQuantity
    {
        private static readonly Pressure _zero = new Pressure();

        public Pressure()
        {
            InKpa = 0;
        }

        private Pressure(double valueInKpa)
        {
            InKpa = valueInKpa;
        }

        [JsonIgnore]
        [XmlIgnore]
        public static Pressure Zero => new Pressure();

        [XmlAttribute]
        public double InKpa { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public double InAtmospheres
        {
            get => InKpa / 101.3;
            set => InKpa = value * 101.3;
        }

        [JsonIgnore]
        [XmlIgnore]
        public double InBars
        {
            get => InKpa * 0.01;
            set => InKpa = value / 0.01;
        }

        [JsonIgnore]
        [XmlIgnore]
        public double InPsi
        {
            get => InKpa * 0.145038;
            set => InKpa = value / 0.145038;
        }

        [JsonIgnore]
        [XmlIgnore]
        public IQuantity ZeroQuantity => _zero;

        [JsonIgnore]
        [XmlIgnore]
        public bool IsZero => InKpa == -1;

        [JsonIgnore]
        [XmlIgnore]
        public double RawValue => InKpa;

        public void UpdateValue(double value, PressureUnits units)
        {
            switch (units)
            {
                case PressureUnits.Kpa:
                    InKpa = value;
                    return;
                case PressureUnits.Atmosphere:
                    InAtmospheres = value;
                    return;
                case PressureUnits.Bar:
                    InBars = value;
                    return;
                case PressureUnits.Psi:
                    InPsi = value;
                    return;
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
        }

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

        public static Pressure FromPsi(double pressureInPsi)
        {
            return new Pressure(pressureInPsi / 0.145038);
        }

        public static Pressure FromAtm(double pressure)
        {
            return new Pressure(pressure * 101.3);
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
    }
}
