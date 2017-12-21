namespace SecondMonitor.DataModel.Drivers
{
    using SecondMonitor.DataModel.BasicProperties;

    public class DriverInfo
    {
        public enum DriverFinishStatus { Na, None, Finished, Dnf, Dnq, Dns, Dq }

        public string DriverName { get; set; }

        public string CarName { get; set; }

        public int CompletedLaps { get; set; }

        public bool InPits { get; set; }

        public bool IsPlayer { get; set; }

        public int Position { get; set; }

        public bool CurrentLapValid { get; set; }

        public Velocity Speed { get; set; } = Velocity.Zero;

        public double LapDistance { get; set; }

        public double TotalDistance { get; set; }

        public double DistanceToPlayer { get; set; }

        public bool IsBeingLappedByPlayer { get; set; } = false;

        public bool IsLappingPlayer { get; set; } = false;

        public DriverFinishStatus FinishStatus { get; set; } = DriverFinishStatus.Na;

        public CarInfo CarInfo { get; set; } = new CarInfo();

        public DriverTimingInfo Timing { get; set; } = new DriverTimingInfo();

        public Point3D WorldPosition { get; set; } = new Point3D(0, 0, 0);

        public DriverDebugInfo DriverDebugInfo { get; } = new DriverDebugInfo();
    }
}
