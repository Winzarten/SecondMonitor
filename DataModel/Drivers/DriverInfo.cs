using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel.Drivers
{
    public class DriverInfo
    {
        public enum DriverFinishStatus { NA, None, Finished, DNF, DNQ, DNS, DQ }
        public string DriverName;
        public string CarName;
        public int CompletedLaps;
        public bool InPits;
        public bool IsPlayer;
        public int Position;
        public bool CurrentLapValid;
        public Single Speed;
        public Single LapDistance;
        public Single TotalDistance;
        public Single DistanceToPlayer;
        public bool IsBeingLappedByPlayer = false;
        public bool IsLapingPlayer = false;
        public DriverFinishStatus FinishStatus = DriverFinishStatus.NA;

        public CarInfo CarInfo = new CarInfo();
        public DriverTimingInfo Timing { get; set; } = new DriverTimingInfo();
    }
}
