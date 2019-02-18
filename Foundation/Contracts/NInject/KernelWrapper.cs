namespace SecondMonitor.Contracts.NInject
{
    using System;
    using System.Reflection;
    using Ninject;
    using Ninject.Parameters;

    public sealed class KernelWrapper
    {
        private readonly IKernel _kernel;

        public  KernelWrapper()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _kernel = BootstrapHelper.LoadNinjectKernel(assemblies);
        }

       public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public T Get<T>(params IParameter[] parameters)
        {
            return _kernel.Get<T>(parameters);
        }
    }
}