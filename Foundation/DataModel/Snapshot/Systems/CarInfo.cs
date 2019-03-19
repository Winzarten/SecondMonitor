namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using System.Xml.Serialization;
    using BasicProperties;

    [Serializable]
    public sealed class CarInfo
    {
        public CarInfo()
        {
            WheelsInfo = new Wheels();
            OilSystemInfo = new OilInfo();
            FuelSystemInfo = new FuelInfo();
            WaterSystemInfo = new WaterInfo();
            Acceleration = new Acceleration();
            FrontHeight = Distance.ZeroDistance;
            RearHeight = Distance.ZeroDistance;
            TurboPressure = Pressure.Zero;
            CarDamageInformation = new CarDamageInformation();
            DrsSystem = new DrsSystem();
            BoostSystem = new BoostSystem();
        }

        public CarDamageInformation CarDamageInformation { get; set; }

        public Wheels WheelsInfo { get; set; }

        public OilInfo OilSystemInfo { get; set; }

        public FuelInfo FuelSystemInfo { get; set; }

        public WaterInfo WaterSystemInfo { get; set; }

        public Acceleration Acceleration { get; set; }

        public Distance FrontHeight { get; set; }

        public Distance RearHeight { get; set; }

        public Pressure TurboPressure { get; set; }

        public bool SpeedLimiterEngaged { get; set; }

        public DrsSystem DrsSystem { get; set; }

        public BoostSystem BoostSystem { get; set; }

        [XmlAttribute]
        public string CurrentGear { get; set; } = string.Empty;

        [XmlAttribute]
        public int EngineRpm { get; set; } = 0;

    }
}
