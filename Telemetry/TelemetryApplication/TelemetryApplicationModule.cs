namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using Controllers;
    using Controllers.MainWindow;
    using Controllers.MainWindow.LapPicker;
    using Controllers.Synchronization;
    using Controllers.TelemetryLoad;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;
    using Repository;
    using Settings;

    public class TelemetryApplicationModule : NinjectModule
    {

        private const string MainWidowScopeName = "MainWindow";

        public override void Load()
        {
            Bind<IMainWindowController>().To<MainWindowController>().DefinesNamedScope(MainWidowScopeName);

            Bind<ISettingsProvider>().To<AppDataSettingsProvider>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetryLoadController>().To<TelemetryLoadController>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetryViewsSynchronization>().To<TelemetryViewsSynchronization>().InNamedScope(MainWidowScopeName);

            Bind<ITelemetryRepositoryFactory>().To<TelemetryRepositoryFactory>();
            Bind<ILapPickerController>().To<LapPickerController>();
        }
    }
}