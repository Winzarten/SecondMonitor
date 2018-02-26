namespace SecondMonitor.DataModel.Summary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Serialization;

    using SecondMonitor.DataModel.BasicProperties;

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

        public Lap BestPersonalLap
        {
            get
            {
                if (this._bestPersonalLap == null)
                {
                    this._bestPersonalLap = FindBest(lap => lap.LapTime, Laps);
                }
                return this._bestPersonalLap;
            }
        }

        public Lap BestSector1Lap
        {
            get
            {
                if (this._bestSector1Lap == null)
                {
                    this._bestSector1Lap = FindBest(lap => lap.Sector1, Laps);
                }
                return this._bestSector1Lap;
            }
        }

        public Lap BestSector2Lap
        {
            get
            {
                if (this._bestSector2Lap == null)
                {
                    this._bestSector2Lap = FindBest(lap => lap.Sector2, Laps);
                }
                return this._bestSector2Lap;
            }
        }

        public Lap BestSector3Lap
        {
            get
            {
                if (this._bestSector3Lap == null)
                {
                    this._bestSector3Lap = FindBest(lap => lap.Sector3, Laps);
                }
                return this._bestSector3Lap;
            }
        }

        private Lap FindBest(Func<Lap, TimeSpan> paramFunc, List<Lap> laps)
        {
            
            return laps.Count == 0 ? null : laps.OrderBy(paramFunc).First();
        }
    }
}