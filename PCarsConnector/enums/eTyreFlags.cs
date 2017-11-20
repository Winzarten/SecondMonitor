using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;



namespace  SecondMonitor.PCarsConnector.enums
{

    public enum ETyreFlags
    {
        TyreAttached = (1 << 0),
        TyreInflated = (1 << 1),
        TyreIsOnGround = (1 << 2),
    }

}
