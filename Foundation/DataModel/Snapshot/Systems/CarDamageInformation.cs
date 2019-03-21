namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using BasicProperties;
    [Serializable]
    public class CarDamageInformation
    {
        public CarDamageInformation()
        {
            Engine = new DamageInformation();
            Transmission = new DamageInformation();
            Suspension = new DamageInformation();
            Bodywork = new DamageInformation();
        }

        public DamageInformation Engine { get; set; }

        public DamageInformation Transmission { get; set; }

        public DamageInformation Suspension { get; set; }

        public DamageInformation Bodywork { get; set; }
    }
}