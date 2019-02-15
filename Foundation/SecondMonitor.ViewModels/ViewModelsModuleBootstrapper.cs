namespace SecondMonitor.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class ViewModelsModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new ViewModelsModule(),}.ToList();
        }
    }
}