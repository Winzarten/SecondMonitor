namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using System.Xml.Serialization;
    using BasicProperties;

    [Serializable]
    public class CarInfo
    {
        public CarInfo()
        {
            WheelsInfo = new Wheels();
            OilSystemInfo = new OilInfo();
            FuelSystemInfo = new FuelInfo();
            WaterSystemInfo = new WaterInfo();
            Acceleration = new Acceleration();
        }

        public Wheels WheelsInfo { get; set; }

        public OilInfo OilSystemInfo { get; set; }

        public FuelInfo FuelSystemInfo { get; set; }

        public WaterInfo WaterSystemInfo { get; set; }

        public Acceleration Acceleration { get; set; }

        [XmlAttribute]
        public string CurrentGear { get; set; } = string.Empty;

        [XmlAttribute]
        public int EngineRpm { get; set; } = 0;

    }
}
