namespace SecondMonitor.Contracts.NInject
{
    using System.Collections.Generic;
    using Ninject.Modules;

    public interface INinjectModuleBootstrapper
    {
        IList<INinjectModule> GetModules();
    }
}