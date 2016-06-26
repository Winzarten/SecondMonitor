namespace SecondMonitor.DataModel.BasicProperties
{
    public class Volume
    {
        private double valueInLiters;

        public Volume()
        {
            valueInLiters = -1;
        }

        private Volume(double valueInLiters)
        {
            this.valueInLiters = valueInLiters;
        }
        public static Volume FromLiters(double volumeInLiters)
        {
            return new Volume(volumeInLiters);
        }

        public double InLiters
        {
            get { return valueInLiters; }
        }
    }
}
