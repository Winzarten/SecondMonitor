namespace SecondMonitor.Telemetry.TelemetryApplication.Factory
{
    using System.Collections.Generic;
    using Ninject;
    using Ninject.Syntax;
    using SecondMonitor.ViewModels;

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        public ViewModelFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public T Create<T>() where T : IViewModel
        {
            return _resolutionRoot.Get<T>();
        }

        public IEnumerable<T> CreateAll<T>() where T : IViewModel
        {
            return _resolutionRoot.GetAll<T>();
        }
    }
}