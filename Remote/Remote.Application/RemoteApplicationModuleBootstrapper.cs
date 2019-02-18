namespace SecondMonitor.Remote.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class RemoteApplicationModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new RemoteApplicationModule(), }.ToList();
        }
    }
}