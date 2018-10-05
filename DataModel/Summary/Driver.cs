namespace SecondMonitor.DataModel.Summary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using BasicProperties;

    [Serializable]
    public class Driver
    {
        private Lap _bestPersonalLap;

        private Lap _bestSector1Lap;

        private Lap _bestSector2Lap;

        private Lap _bestSector3Lap;

        public string DriverName { get; set; }

        public int FinishingPosition { get; set; }

        public string CarName { get; set; }

        public int TotalLaps { get; set; }

        public Velocity TopSpeed { get; set; } = Velocity.Zero;

        public List<Lap> Laps { get; } = new List<Lap>();

        public bool IsPlayer { get; set;  }

        public bool Finished { get; set; } = true;

        [XmlIgnore]
        public Lap BestPersonalLap
        {
            get
            {
                if (_bestPersonalLap == null)
                {
                    _bestPersonalLap = FindBest(lap => lap.LapTime, Laps);
                }
                return _bestPersonalLap;
            }
        }

        [XmlIgnore]
        public Lap BestSector1Lap
        {
            get
            {
                if (_bestSector1Lap == null)
                {
                    _bestSector1Lap = FindBest(lap => lap.Sector1, Laps);
                }
                return _bestSector1Lap;
            }
        }

        [XmlIgnore]
        public Lap BestSector2Lap
        {
            get
            {
                if (_bestSector2Lap == null)
                {
                    _bestSector2Lap = FindBest(lap => lap.Sector2, Laps);
                }
                return _bestSector2Lap;
            }
        }

        [XmlIgnore]
        public Lap BestSector3Lap
        {
            get
            {
                if (_bestSector3Lap == null)
                {
                    _bestSector3Lap = FindBest(lap => lap.Sector3, Laps);
                }
                return _bestSector3Lap;
            }
        }

        private Lap FindBest(Func<Lap, TimeSpan> paramFunc, List<Lap> laps)
        {

            return laps.Count == 0 ? null : laps.Where(l => l.IsValid  && paramFunc(l) != TimeSpan.Zero).OrderBy(paramFunc).First();
        }
    }
}