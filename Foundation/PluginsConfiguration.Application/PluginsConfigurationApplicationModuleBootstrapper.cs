namespace SecondMonitor.PluginsConfiguration.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class PluginsConfigurationApplicationModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new PluginsConfigurationApplicationModule()}.ToList();
        }
    }
}