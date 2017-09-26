using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondMonitor.DataModel
{
    public class SessionInfo
    {
        public enum SessionTypeEnum { NA, Practice, Qualification, WarmUp, Race}

        public TimeSpan SessionTime;
        public bool IsActive;
        public string TrackName;
        public string TrackLayoutName;        

        public SessionTypeEnum SessionType;

        public SessionInfo()
        {
            SessionTime = new TimeSpan(0);
            TrackName = "";
            TrackLayoutName = "";
        }
    }
}
