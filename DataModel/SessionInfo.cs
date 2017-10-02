using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecondMonitor.DataModel
{
    public class SessionInfo
    {
        public enum SessionTypeEnum { NA, Practice, Qualification, WarmUp, Race}
        public enum SessionPhaseEnum { Countdown, Green, Checkered}

        public TimeSpan SessionTime;
        public bool IsActive;
        public string TrackName;
        public string TrackLayoutName;
        public Single LayoutLength;

        public SessionTypeEnum SessionType;
        public SessionPhaseEnum SessionPhase;

        public SessionInfo()
        {
            SessionTime = new TimeSpan(0);
            TrackName = "";
            TrackLayoutName = "";
        }
    }
}
