namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using System.Collections.Generic;
    using Contracts.NInject;
    using Ninject.Modules;

    public class TelemetryApplicationModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new List<INinjectModule>(new []{new TelemetryApplicationModule()});
        }
    }
}