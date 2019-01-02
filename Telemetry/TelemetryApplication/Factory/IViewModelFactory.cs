namespace SecondMonitor.Telemetry.TelemetryApplication.Factory
{
    using System.Collections.Generic;
    using SecondMonitor.ViewModels;

    public interface IViewModelFactory
    {
        T Create<T>() where T : IViewModel;

        IEnumerable<T> CreateAll<T>() where T : IViewModel;
    }
}