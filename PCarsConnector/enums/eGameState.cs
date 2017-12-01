using System.ComponentModel;

namespace  SecondMonitor.PCarsConnector.enums
{
    //public class eGameState {public eGameState eGameState { get; set; }}

    public enum EGameState
    {
        [Description("Waiting for game to start...")]
        GameExited = 0,
        [Description("In Menus")]
        GameFrontEnd,
        [Description("In Session")]
        GameIngamePlaying,
        [Description("Game Paused")]
        GameIngamePaused,
        //-------------
        GameMax
    }
}
