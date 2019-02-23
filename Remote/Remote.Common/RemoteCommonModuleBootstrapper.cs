namespace SecondMonitor.Remote.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class RemoteCommonModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new RemoteCommonModule(), }.ToList();
        }
    }
}