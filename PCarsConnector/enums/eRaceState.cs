using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum ERaceState
    {
        [Description("Invalid")]
        RacestateInvalid = 0,
        [Description("Not started")]
        RacestateNotStarted,
        [Description("Racing")]
        RacestateRacing,
        [Description("Finished")]
        RacestateFinished,
        [Description("Disqualified")]
        RacestateDisqualified,
        [Description("Retired")]
        RacestateRetired,
        [Description("DNF")]
        RacestateDnf,
        //-------------
        RacestateMax
    }
}
