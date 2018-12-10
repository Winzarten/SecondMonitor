using System;

namespace SecondMonitor.DataModel.BasicProperties
{
    [Serializable]
    public class TyreWear
    {
        public double ActualWear { get; set; }
        public double NoWearWearLimit { get; set; }
        public double LightWearLimit { get; set; }
        public double HeavyWearLimit { get; set; }
    }
}