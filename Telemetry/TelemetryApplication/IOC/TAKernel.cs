namespace SecondMonitor.Telemetry.TelemetryApplication.IOC
{
    using System;
    using Ninject;

    public sealed class TaKernel
    {
        private readonly IKernel _kernel;
        private static readonly Lazy<TaKernel> _instance = new Lazy<TaKernel>(() => new TaKernel());


        public static TaKernel Instance => _instance.Value;

        private TaKernel()
        {
            _kernel = new StandardKernel(new TelemetryApplicationModule());
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }
    }
}