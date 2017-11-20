using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum ECrashDamageState
    {
        CrashDamageNone = 0,
        CrashDamageOfftrack,
        CrashDamageLargeProp,
        CrashDamageSpinning,
        CrashDamageRolling,
        //-------------
        CrashMax
    }
}
