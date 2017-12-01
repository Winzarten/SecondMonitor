using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EPitMode
    {
        [Description("None")]
        PitModeNone = 0,
        [Description("Pit Entry")]
        PitModeDrivingIntoPits,
        [Description("In Pits")]
        PitModeInPit,
        [Description("Pit Exit")]
        PitModeDrivingOutOfPits,
        [Description("Pit Garage")]
        PitModeInGarage,
        //-------------
        PitModeMax
    }
}
