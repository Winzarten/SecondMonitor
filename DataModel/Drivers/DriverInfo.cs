using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.DataModel.Drivers
{
    public class DriverInfo
    {
        public string DriverName;
        public string CarName;
        public int CompletedLaps;
        public bool InPits;
        public bool IsPlayer;
        public int Position;
        public bool CurrentLapValid;
        public Single Speed;
    }
}
