using System;
using Newtonsoft.Json;

namespace SecondMonitor.DataModel
{
    public class Temperature
    {
        private double _valueInCelsius;

        public Temperature()
        {
            _valueInCelsius = -1;
        }

        private Temperature(double valueInCelsius)
        {
            this._valueInCelsius = valueInCelsius;
        }

        public double InCelsius
        {
            get => _valueInCelsius;
        }
        [JsonIgnore]
        public double InFahrenheit
        {
            get => (_valueInCelsius * 9) / 5 + 32; 
        }
        [JsonIgnore]
        public double InKelvin
        {
            get => _valueInCelsius + 273.15;
        }

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

        static public string GetUnitSymbol(TemperatureUnits units)
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

            return Comparison(temp1, temp2) == 0;

        }

        public static bool operator !=(Temperature temp1, Temperature temp2)
        {

            return Comparison(temp1, temp2) != 0;

        }

        public override bool Equals(object obj)
        {

            if (!(obj is Temperature)) return false;

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

            if (temp1.InCelsius < temp2.InCelsius)

                return -1;

            else if (temp1.InCelsius == temp2.InCelsius)

                return 0;

            else if (temp1.InCelsius > temp2.InCelsius)

                return 1;

            return 0;

        }

        public override int GetHashCode()
        {
            var hashCode = 1054699813;
            hashCode = hashCode * -1521134295 + _valueInCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InFahrenheit.GetHashCode();
            return hashCode;
        }        
    }
}
