namespace SecondMonitor.DataModel
{
    public class Temperature
    {
        public enum TemperatureUnits { Ceslius, Fahrenheit };
        private double valueInCelsius;

        public Temperature()
        {
            valueInCelsius = -1;
        }

        private Temperature(double valueInCelsius)
        {
            this.valueInCelsius = valueInCelsius;
        }

        public double InCelsius
        {
            get { return valueInCelsius; }
        }
        public double InFahrenheit
        {
            get { return (valueInCelsius * 9) / 5 + 32; ; }
        }

        public static Temperature FromCelsius(double temperatureInCelsius)
        {
            return new Temperature(temperatureInCelsius);
        }
        public static Temperature FromKelvin(double temperetureInKelvin)
        {
            return new Temperature( temperetureInKelvin - 273.15);
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
            hashCode = hashCode * -1521134295 + valueInCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InCelsius.GetHashCode();
            hashCode = hashCode * -1521134295 + InFahrenheit.GetHashCode();
            return hashCode;
        }        
    }
}
