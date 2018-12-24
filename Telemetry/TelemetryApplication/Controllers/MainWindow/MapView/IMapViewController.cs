namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.MapView
{
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels.MapView;

    public interface IMapViewController : IController
    {
        IMapViewViewModel MapViewViewModel { get; set; }
    }
}