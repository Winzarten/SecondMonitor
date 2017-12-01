using SecondMonitor.DataModel.BasicProperties;
using System;

namespace SecondMonitor.DataModel.Drivers
{
    public class DriverInfo
    {
        public enum DriverFinishStatus { Na, None, Finished, Dnf, Dnq, Dns, Dq }
        public string DriverName;
        public string CarName;
        public int CompletedLaps;
        public bool InPits;
        public bool IsPlayer;
        public int Position;
        public bool CurrentLapValid;
        public Velocity Speed= Velocity.Zero;
        public Single LapDistance;
        public Single TotalDistance;
        public Single DistanceToPlayer;
        public bool IsBeingLappedByPlayer = false;
        public bool IsLapingPlayer = false;
        public DriverFinishStatus FinishStatus = DriverFinishStatus.Na;

        public CarInfo CarInfo = new CarInfo();
        public DriverTimingInfo Timing { get; set; } = new DriverTimingInfo();
        public Point3D WorldPostion { get; set; } = new Point3D(0, 0, 0);

        public DriverDebugInfo DriverDebugInfo { get; } = new DriverDebugInfo();
    }
}
