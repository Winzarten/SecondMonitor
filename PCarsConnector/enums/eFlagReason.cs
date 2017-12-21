using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum EFlagReason
    {
        [Description("No Reason")]
        FlagReasonNone = 0,

        [Description("Solo Crash")]
        FlagReasonSoloCrash,

        [Description("Vehicle Crash")]
        FlagReasonVehicleCrash,

        [Description("Vehicle Obstruction")]
        FlagReasonVehicleObstruction,

        // -------------
        FlagReasonMax
    }
}
