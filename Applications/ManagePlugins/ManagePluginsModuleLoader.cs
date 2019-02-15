namespace SecondMonitor.ManagePlugins
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class ManagePluginsModuleLoader : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new ManagePluginsModule()}.ToList();
        }
    }
}