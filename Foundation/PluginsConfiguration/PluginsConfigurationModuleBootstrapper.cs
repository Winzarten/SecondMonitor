namespace SecondMonitor.PluginsConfiguration.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class PluginsConfigurationModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new PluginsConfigurationModule()}.ToList();
        }
    }
}