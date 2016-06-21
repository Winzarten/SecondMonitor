using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecondWindow.Core.R3EConnector;

namespace SecondWindow.Core.PluginManager
{
    public class PluginManager
    {        
        public PluginManager(IR3EConnector connector)
        {
            Connector = connector;
        }

        public IR3EConnector Connector
        {
            get;
            private set;
        }
    }
}
