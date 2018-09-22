namespace SecondMonitor.Timing.Controllers
{
    using AutoUpdaterDotNET;

    public class AutoUpdateController
    {
        public void CheckForUpdate()
        {
            AutoUpdater.Start("https://raw.githubusercontent.com/Winzarten/SecondMonitor/master/AutoUpdater.xml");
        }
    }
}