using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EPitMode
    {
        [Description("None")]
        PitModeNone = 0,
        [Description("Pit Entry")]
        PitModeDrivingIntoPits,
        [Description("In Pits")]
        PitModeInPit,
        [Description("Pit Exit")]
        PitModeDrivingOutOfPits,
        [Description("Pit Garage")]
        PitModeInGarage,
        //-------------
        PitModeMax
    }
}
