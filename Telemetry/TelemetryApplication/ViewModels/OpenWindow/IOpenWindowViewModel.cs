namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.OpenWindow
{
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public interface IOpenWindowViewModel : IViewModel
    {
        ICommand RefreshRecentCommand { get; set; }
    }
}