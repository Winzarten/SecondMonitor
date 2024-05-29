namespace SecondMonitor.Contracts.NInject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Ninject;

    public class BootstrapHelper
    {
        public static StandardKernel LoadNinjectKernel(IEnumerable<Assembly> assemblies)
        {
            var standardKernel = new StandardKernel();
            foreach (Assembly assembly in assemblies)
            {
                assembly
                    .GetTypes()
                    .Where(t =>
                        t.GetInterfaces()
                            .Any(i =>
                                i.Name == typeof(INinjectModuleBootstrapper).Name))
                    .ToList()
                    .ForEach(t =>
                    {
                        var ninjectModuleBootstrapper =
                            (INinjectModuleBootstrapper)Activator.CreateInstance(t);

                        standardKernel.Load(ninjectModuleBootstrapper.GetModules().Where(x => !standardKernel.HasModule(x.Name)));
                    });
            }
            return standardKernel;
        }
    }
}