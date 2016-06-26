namespace SecondMonitor.DataModel
{
    public class Temperature
    {
        
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

        public static Temperature FromCelsius(double temperatureInCelsius)
        {
            return new Temperature(temperatureInCelsius);
        }
        public static Temperature FromKelvin(double temperetureInKelvin)
        {
            return new Temperature( temperetureInKelvin - 273.15);
        }
    }
}
