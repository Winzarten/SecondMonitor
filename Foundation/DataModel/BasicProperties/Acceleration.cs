namespace SecondMonitor.DataModel.BasicProperties
{
    using System;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public class Acceleration
    {

        private static readonly double GConst = 9.8;

        public Acceleration()
        {

        }

        [JsonIgnore]
        [XmlIgnore]
        public double XinG
        {
            get => XinMs / GConst;
            set => XinMs = value * GConst;
        }

        [JsonIgnore]
        [XmlIgnore]
        public double YinG
        {
            get => YinMs / GConst;
            set => YinMs = value * GConst;
        }

        [JsonIgnore]
        [XmlIgnore]
        public double ZinG
        {
            get => ZinMs / GConst;
            set => ZinMs = value * GConst;
        }

        public double XinMs { get; set; }

        public double YinMs { get; set; }

        public double ZinMs { get; set; }
    }
}
