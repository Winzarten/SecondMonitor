namespace SecondMonitor.Rating.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.NInject;
    using Ninject.Modules;

    public class RatingCommonModuleBootstrapper : INinjectModuleBootstrapper
    {
        public IList<INinjectModule> GetModules()
        {
            return new INinjectModule[] {new RatingCommonModule(), }.ToList();
        }
    }
}