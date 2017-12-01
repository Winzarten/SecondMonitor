using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EFlagColors
    {
        [Description("No Flag")]
        FlagColourNone = 0,       // Not used for actual flags, only for some query functions
        [Description("Green Flag")]
        FlagColourGreen,          // End of danger zone, or race started
        [Description("Blue Flag")]
        FlagColourBlue,           // Faster car wants to overtake the participant
        [Description("White Flag")]
        FlagColourWhite,          // Approaching a slow car
        [Description("Yellow Flag")]
        FlagColourYellow,         // Danger on the racing surface itself
        [Description("Double Yellow Flag")]
        FlagColourDoubleYellow,  // Danger that wholly or partly blocks the racing surface
        [Description("Black Flag")]
        FlagColourBlack,          // Participant disqualified
        [Description("Chequered Flag")]
        FlagColourChequered,      // Chequered flag
        //-------------
        FlagColourMax
    }
}
