namespace SecondMonitor.Telemetry.TelemetryApplication.Factory
{
    using IOC;
    using SecondMonitor.ViewModels;

    public class ViewModelFactory : IViewModelFactory
    {
        public T Create<T>() where T : IAbstractViewModel
        {
            return TaKernel.Instance.Get<T>();
        }
    }
}