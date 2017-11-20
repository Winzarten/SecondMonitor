using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    [Flags]
    public enum ECarFlags
    {
        [Description("None")]
        None = 0,
        [Description("Headlight")]
        CarHeadlight = 1,
        [Description("Engine Active")]
        CarEngineActive = 2,
        [Description("Engine Warning")]
        CarEngineWarning = 4,
        [Description("Speed Limiter")]
        CarSpeedLimiter = 8,
        [Description("ABS")]
        CarAbs = 16,
        [Description("Handbrake")]
        CarHandbrake = 32
    }

}
