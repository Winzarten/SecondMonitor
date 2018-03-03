namespace SecondMonitor.DataModel.Summary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;

    public class SessionSummary
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

        public Lap SessionBestLap
        {
            get
            {
                if (this._sessionBestLaps == null)
                {
                    this._sessionBestLaps = FindBest((driver) => driver.BestPersonalLap?.LapTime ?? TimeSpan.MaxValue, d => d.BestPersonalLap, Drivers);
                }
                return this._sessionBestLaps;
            }
        }

        public Lap SessionBestSector1
        {
            get
            {
                if (this._sessionBestSector1 == null)
                {
                    this._sessionBestSector1 = FindBest((driver) => driver.BestSector1Lap?.Sector1 ?? TimeSpan.MaxValue, d => d.BestSector1Lap, Drivers);
                }
                return this._sessionBestSector1;
            }
        }

        public Lap SessionBestSector2
        {
            get
            {
                if (this._sessionBestSector2 == null)
                {
                    this._sessionBestSector2 = FindBest((driver) => driver.BestSector2Lap?.Sector2 ?? TimeSpan.MaxValue, d => d.BestSector2Lap, Drivers);
                }
                return this._sessionBestSector2;
            }
        }

        public Lap SessionBestSector3
        {
            get
            {
                if (this._sessionBestSector3 == null)
                {
                    this._sessionBestSector3 = FindBest((driver) => driver.BestSector3Lap?.Sector3 ?? TimeSpan.MaxValue, d => d.BestSector3Lap, Drivers);
                }
                return this._sessionBestSector3;
            }
        }


        private Lap FindBest(Func<Driver, TimeSpan> comparisonFunc, Func<Driver,Lap> resultFunc, List<Driver> drivers)
        {
            return drivers.Count == 0 ? null : resultFunc(drivers.OrderBy(comparisonFunc).First());
        }

    }
}