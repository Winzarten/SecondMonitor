using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum ESessionState
    {
        [Description("No Session")]
        SessionInvalid = 0,
        [Description("Practise")]
        SessionPractice,
        [Description("Testing")]
        SessionTest,
        [Description("Qualifying")]
        SessionQualify,
        [Description("Formation Lap")]
        SessionFormationlap,
        [Description("Racing")]
        SessionRace,
        [Description("Time Trial")]
        SessionTimeAttack,
        //-------------
        SessionMax
    }
}
