namespace SecondMonitor.DataModel.Summary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using BasicProperties;
    using Snapshot;

    [Serializable]
    public sealed class SessionSummary
    {
        private Lap _sessionBestLaps;
        private Lap _sessionBestSector1;
        private Lap _sessionBestSector2;
        private Lap _sessionBestSector3;


        public DateTime SessionRunTime { get; set; } = DateTime.Now;

        public SessionType SessionType { get; set; }

        public string Simulator { get; set; }

        public SessionLengthType SessionLengthType { get; set; } = SessionLengthType.Na;

        public int TotalNumberOfLaps { get; set; } = 0;

        public TimeSpan SessionLength { get; set; }

        public TrackInfo TrackInfo { get; set; } = new TrackInfo();

        public List<Driver> Drivers { get; } = new List<Driver>();

        [XmlIgnore]
        public Lap SessionBestLap
        {
            get
            {
                if (_sessionBestLaps == null)
                {
                    _sessionBestLaps = FindBest((driver) => driver.BestPersonalLap?.LapTime ?? TimeSpan.MaxValue, d => d.BestPersonalLap, Drivers);
                }
                return _sessionBestLaps;
            }
        }

        [XmlIgnore]
        public Lap SessionBestSector1
        {
            get
            {
                if (_sessionBestSector1 == null)
                {
                    _sessionBestSector1 = FindBest((driver) => driver.BestSector1Lap?.Sector1 ?? TimeSpan.MaxValue, d => d.BestSector1Lap, Drivers);
                }
                return _sessionBestSector1;
            }
        }

        [XmlIgnore]
        public Lap SessionBestSector2
        {
            get
            {
                if (_sessionBestSector2 == null)
                {
                    _sessionBestSector2 = FindBest((driver) => driver.BestSector2Lap?.Sector2 ?? TimeSpan.MaxValue, d => d.BestSector2Lap, Drivers);
                }
                return _sessionBestSector2;
            }
        }

        [XmlIgnore]
        public Lap SessionBestSector3
        {
            get
            {
                if (_sessionBestSector3 == null)
                {
                    _sessionBestSector3 = FindBest((driver) => driver.BestSector3Lap?.Sector3 ?? TimeSpan.MaxValue, d => d.BestSector3Lap, Drivers);
                }
                return _sessionBestSector3;
            }
        }

        private static Lap FindBest(Func<Driver, TimeSpan> comparisonFunc, Func<Driver,Lap> resultFunc, List<Driver> drivers)
        {
            return drivers.Count == 0 ? null : resultFunc(drivers.OrderBy(comparisonFunc).First());
        }

    }
}