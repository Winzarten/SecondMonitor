using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EPitSchedule
    {
        [Description("None")]
        PitScheduleNone = 0,        // Nothing scheduled
        [Description("Standard")]
        PitScheduleStandard,        // Used for standard pit sequence
        [Description("Drive Through")]
        PitScheduleDriveThrough,   // Used for drive-through penalty
        [Description("Stop Go")]
        PitScheduleStopGo,         // Used for stop-go penalty
        //-------------
        PitScheduleMax
    }
}
