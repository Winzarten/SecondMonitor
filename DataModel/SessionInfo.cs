using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondMonitor.DataModel
{
    public class SessionInfo
    {        
       public TimeSpan SessionTime;

        public SessionInfo()
        {
            SessionTime = new TimeSpan(0);
        }
    }
}
