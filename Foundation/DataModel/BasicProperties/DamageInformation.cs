namespace SecondMonitor.DataModel.BasicProperties
{
    public class DamageInformation
    {
        public DamageInformation()
        {
            Damage = 0;
            MediumDamageThreshold = 0.05;
            HeavyDamageThreshold = 0.25;
        }

        public double Damage { get; set; }

        public double MediumDamageThreshold { get; set; }

        public double HeavyDamageThreshold { get; set; }
    }
}