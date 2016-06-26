namespace SecondMonitor.DataModel
{
    public class Pressure
    {
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

        public static Pressure FromKiloPascals(double pressureInKpa)
        {
            return new Pressure(pressureInKpa);
        }
    }
}
