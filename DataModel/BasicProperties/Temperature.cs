namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public class Temperature : IQuantity
    {
        public static Temperature Zero = new Temperature();
        private readonly bool _isZero;

        public Temperature()
        {
            InCelsius = -1;
            _isZero = true;
        }

        private Temperature(double valueInCelsius)
        {
            InCelsius = valueInCelsius;
        }

        [XmlAttribute]
        public double InCelsius { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public double InFahrenheit => ((InCelsius * 9) / 5) + 32;

        [JsonIgnore]
        [XmlIgnore]
        public double InKelvin => InCelsius + 273.15;

        [JsonIgnore]
        [XmlIgnore]
        public IQuantity ZeroQuantity => Zero;

        [JsonIgnore]
        [XmlIgnore]
        public bool IsZero => _isZero;

        [JsonIgnore]
        [XmlIgnore]
        public double RawValue => InCelsius;

        public static Temperature FromCelsius(double temperatureInCelsius)
        {
            return new Temperature(temperatureInCelsius);
        }

        public static Temperature FromKelvin(double temperetureInKelvin)
        {
            return new Temperature( temperetureInKelvin - 273.15);
        }

        public double GetValueInUnits(TemperatureUnits units)
        {
            switch (units)
            {
                case TemperatureUnits.Celsius:
                    return InCelsius;
                case TemperatureUnits.Fahrenheit:
                    return InFahrenheit;
                case TemperatureUnits.Kelvin:
                    return InKelvin;
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        public string GetValueInUnits(TemperatureUnits units, int decimalPlaces)
        {
            switch (units)
            {
                case TemperatureUnits.Celsius:
                    return InCelsius.ToString($"F{decimalPlaces}");
                case TemperatureUnits.Fahrenheit:
                    return InFahrenheit.ToString($"F{decimalPlaces}");
                case TemperatureUnits.Kelvin:
                    return InKelvin.ToString($"F{decimalPlaces}");
            }
            throw new ArgumentException("Unable to return value in" + units.ToString());
        }

        public string GetFormattedWithUnits(int decimalPlaces, TemperatureUnits temperatureUnits)
        {
            return GetValueInUnits(temperatureUnits).ToString($"F{decimalPlaces}") + " "
                                                                                   + GetUnitSymbol(temperatureUnits);
        }

        public static string GetUnitSymbol(TemperatureUnits units)
        {
            switch (units)
            {
                case TemperatureUnits.Celsius:
                    return "°C";
                case TemperatureUnits.Fahrenheit:
                    return "°F";
                case TemperatureUnits.Kelvin:
                    return "K";
            }
            throw new ArgumentException("Unable to return symbol fir" + units.ToString());
        }

        public static bool operator <(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) < 0;

        }

        public static bool operator >(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) > 0;

        }

        public static bool operator ==(Temperature temp1, Temperature temp2)
        {
            if (ReferenceEquals(temp1, temp2))
            {
                return true;
            }

            if (ReferenceEquals(temp1, null))
            {
                return false;
            }

            if (ReferenceEquals(temp2, null))
            {
                return false;
            }

            return temp1._isZero == temp2._isZero &&  Comparison(temp1, temp2) == 0;

        }

        public static bool operator !=(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) != 0;

        }

        public override bool Equals(object obj)
        {
            if (!(obj is Temperature))
            {
                return false;
            }

            return this == (Temperature)obj;

        }

        public static bool operator <=(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) <= 0;

        }

        public static bool operator >=(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) >= 0;

        }

        public static int Comparison(Temperature temp1, Temperature temp2)
        {
            if (temp1 == null || temp2 == null)
            {
                return -1;
            }

            if (temp1.InCelsius < temp2.InCelsius)
            {
                return -1;
            }
            else if (temp1.InCelsius == temp2.InCelsius)
            {
                return 0;
            }
            else if (temp1.InCelsius > temp2.InCelsius)
            {

                return 1;
            }
            return 0;

        }

        public override int GetHashCode()
        {
            var hashCode = 1054699813;
            hashCode = hashCode * -1521134295 + InCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InFahrenheit.GetHashCode();
            return hashCode;
        }
    }
}
