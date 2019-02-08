namespace SecondMonitor.Contracts.NInject
{
    using System;
    using System.Reflection;
    using Ninject;

    public sealed class KernelWrapper
    {
        private readonly IKernel _kernel;
        private static readonly Lazy<KernelWrapper> _instance = new Lazy<KernelWrapper>(() => new KernelWrapper());


        public static KernelWrapper Instance => _instance.Value;

        private KernelWrapper()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _kernel = BootstrapHelper.LoadNinjectKernel(assemblies);
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }
    }
}