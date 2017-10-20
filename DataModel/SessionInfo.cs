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
        public enum SessionLengthTypeEnum { NA, Laps, Time}

        public TimeSpan SessionTime;
        public bool IsActive;
        public string TrackName;
        public string TrackLayoutName;
        public Single LayoutLength;

        public SessionTypeEnum SessionType;
        public SessionPhaseEnum SessionPhase;
        public SessionLengthTypeEnum SessionLengthType = SessionInfo.SessionLengthTypeEnum.NA;
        public Single SessionTimeRemaining = 0;
        public int TotalNumberOfLaps = 0;
        public int LeaderCurrentLap;
        public WeatherInfo WeatherInfo = new WeatherInfo();

        public SessionInfo()
        {
            SessionTime = new TimeSpan(0);
            TrackName = "";
            TrackLayoutName = "";
        }
    }
}
