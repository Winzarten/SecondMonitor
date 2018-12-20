namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using Controllers;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    public class TelemetryApplicationModule : NinjectModule
    {

        private const string MainWidowScopeName = "MainWindow";

        public override void Load()
        {
            Bind<IMainWindowController>().To<MainWindowController>().DefinesNamedScope(MainWidowScopeName);
        }
    }
}