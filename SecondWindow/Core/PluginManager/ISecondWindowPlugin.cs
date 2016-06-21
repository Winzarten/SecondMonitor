using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondWindow.Core.PluginManager
{
    public interface ISecondWindowPlugin
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
