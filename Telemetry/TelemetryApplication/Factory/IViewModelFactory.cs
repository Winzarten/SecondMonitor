namespace SecondMonitor.Telemetry.TelemetryApplication.Factory
{
    using SecondMonitor.ViewModels;

    public interface IViewModelFactory
    {
        T Create<T>() where T : IAbstractViewModel;
    }
}