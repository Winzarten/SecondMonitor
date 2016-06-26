using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondMonitor.Core.PluginManager
{
    public interface ISecondMonitorPlugin
    {
        PluginManager PluginManager
        {
            get;
            set;
        }

        void RunPlugin();
        bool IsDaemon
        {
            get;
        }
    }
}
