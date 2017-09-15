using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class Driver
    {
        public Driver(bool isPlayer, string name, int postion)
        {
            IsPlayer = isPlayer;
            Name = name;
            Position = postion;
        }

        public bool IsPlayer { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        public static Driver FromModel(DriverInfo modelDriverInfo)
        {
            return new Driver(modelDriverInfo.IsPlayer, modelDriverInfo.DriverName, modelDriverInfo.Position);
        }
    }
}
