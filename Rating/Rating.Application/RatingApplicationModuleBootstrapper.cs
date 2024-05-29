namespace SecondMonitor.Rating.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class RatingApplicationModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new RatingApplicationModule() }.ToList();
        }
    }
}