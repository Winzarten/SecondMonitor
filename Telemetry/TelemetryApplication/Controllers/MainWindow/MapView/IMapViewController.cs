namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.MapView
{
    using System;
    using SecondMonitor.ViewModels.Controllers;
    using ViewModels.MapView;

    public interface IMapViewController : IController
    {
        IMapViewViewModel MapViewViewModel { get; set; }
    }
}