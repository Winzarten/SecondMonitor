using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EFlagReason
    {
        [Description("No Reason")]
        FlagReasonNone = 0,
        [Description("Solo Crash")]
        FlagReasonSoloCrash,
        [Description("Vehicle Crash")]
        FlagReasonVehicleCrash,
        [Description("Vehicle Obstruction")]
        FlagReasonVehicleObstruction,
        //-------------
        FlagReasonMax
    }
}
