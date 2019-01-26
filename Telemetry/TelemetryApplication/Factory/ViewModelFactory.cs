namespace SecondMonitor.Telemetry.TelemetryApplication.Factory
{
    using System.Collections.Generic;
    using IOC;
    using SecondMonitor.ViewModels;

    public class ViewModelFactory : IViewModelFactory
    {
        public T Create<T>() where T : IViewModel
        {
            return TaKernel.Instance.Get<T>();
        }

        public IEnumerable<T> CreateAll<T>() where T : IViewModel
        {
            return TaKernel.Instance.GetAll<T>();
        }
    }
}