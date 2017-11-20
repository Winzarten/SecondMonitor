using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum ERaceState
    {
        [Description("Invalid")]
        RacestateInvalid = 0,
        [Description("Not started")]
        RacestateNotStarted,
        [Description("Racing")]
        RacestateRacing,
        [Description("Finished")]
        RacestateFinished,
        [Description("Disqualified")]
        RacestateDisqualified,
        [Description("Retired")]
        RacestateRetired,
        [Description("DNF")]
        RacestateDnf,
        //-------------
        RacestateMax
    }
}
