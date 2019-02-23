namespace SecondMonitor.ViewModels.Controllers
{
    using AutoUpdaterDotNET;

    public class AutoUpdateController
    {
        public void CheckForUpdate()
        {
            #if! DEBUG
            AutoUpdater.Start("https://raw.githubusercontent.com/Winzarten/SecondMonitor/master/AutoUpdater.xml");
            #endif
        }
    }
}