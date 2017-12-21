namespace SecondMonitor.DataModel.BasicProperties
{
    using Newtonsoft.Json;

    public class Acceleration
    {

        private static readonly double GConst = 9.8;

        [JsonIgnore]
        public double XinG
        {
            get => XinMs / GConst;
            set => XinMs = value * GConst;
        }

        [JsonIgnore]
        public double YinG
        {
            get => YinMs / GConst;
            set => YinMs = value * GConst;
        }

        [JsonIgnore]
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
