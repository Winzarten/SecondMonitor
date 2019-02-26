using System.Diagnostics;

namespace SecondMonitor.DataModel.Snapshot.Drivers
{
    using System;

    using BasicProperties;

    using Systems;

    [Serializable]
    [DebuggerDisplay("Driver Name: {DriverName}")]
    public sealed class DriverInfo : IDriverInfo
    {
        public DriverInfo()
        {

        }

        public string DriverName { get; set; }

        public string CarName { get; set; }

        public string CarClassName { get; set; }

        public string CarClassId { get; set; }

        public int CompletedLaps { get; set; }

        public bool InPits { get; set; }

        public bool IsPlayer { get; set; }

        public int Position { get; set; }

        public int PositionInClass { get; set; }

        public bool CurrentLapValid { get; set; }

        public double LapDistance { get; set; }

        public double TotalDistance { get; set; }

        public double DistanceToPlayer { get; set; }

        public bool IsBeingLappedByPlayer { get; set; } = false;

        public bool IsLappingPlayer { get; set; } = false;

        public DriverFinishStatus FinishStatus { get; set; } = DriverFinishStatus.Na;

        public CarInfo CarInfo { get; set; } = new CarInfo();

        public DriverTimingInfo Timing { get; set; } = new DriverTimingInfo();

        public Point3D WorldPosition { get; set; } = new Point3D(Distance.ZeroDistance, Distance.ZeroDistance, Distance.ZeroDistance);

        public DriverDebugInfo DriverDebugInfo { get; } = new DriverDebugInfo();

        public Velocity Speed { get; set; } = Velocity.Zero;
    }
}
