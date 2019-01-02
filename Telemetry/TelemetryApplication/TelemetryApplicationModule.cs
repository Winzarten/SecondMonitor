namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using Controllers.MainWindow;
    using Controllers.MainWindow.GraphPanel;
    using Controllers.MainWindow.LapPicker;
    using Controllers.MainWindow.MapView;
    using Controllers.MainWindow.Replay;
    using Controllers.MainWindow.Snapshot;
    using Controllers.Synchronization;
    using Controllers.TelemetryLoad;
    using Factory;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;
    using Repository;
    using SecondMonitor.ViewModels.Colors;
    using Settings;
    using SimdataManagement;
    using TelemetryManagement.StoryBoard;
    using ViewModels;
    using ViewModels.GraphPanel;
    using ViewModels.LapPicker;
    using ViewModels.MapView;
    using ViewModels.Replay;
    using ViewModels.SnapshotSection;

    public class TelemetryApplicationModule : NinjectModule
    {

        private const string MainWidowScopeName = "MainWindow";

        public override void Load()
        {
            Bind<IMainWindowController>().To<MainWindowController>().DefinesNamedScope(MainWidowScopeName);
            Bind<TelemetryStoryBoardFactory>().ToSelf();
            Bind<IMapsLoaderFactory>().To<MapsLoaderFactory>();

            Bind<ISettingsProvider>().To<AppDataSettingsProvider>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetryLoadController>().To<TelemetryLoadController>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetryViewsSynchronization>().To<TelemetryViewsSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IMainWindowViewModel>().To<MainWindowViewModel>().InNamedScope(MainWidowScopeName);
            Bind<IMapViewController>().To<MapViewController>().InNamedScope(MainWidowScopeName);
            Bind<ILapColorSynchronization>().To<LapColorSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IColorPaletteProvider>().To<BasicColorPaletteProvider>().InNamedScope(MainWidowScopeName);
            Bind<IReplayController>().To<ReplayController>();
            Bind<ISnapshotSectionController>().To<SnapshotSectionController>();

            Bind<ITelemetryRepositoryFactory>().To<TelemetryRepositoryFactory>();
            Bind<ILapPickerController>().To<LapPickerController>();
            Bind<IViewModelFactory>().To<ViewModelFactory>();
            Bind<ISnapshotSectionViewModel>().To<SnapshotSectionViewModel>();

            Bind<ILapSelectionViewModel>().To<LapSelectionViewModel>();
            Bind<ILapSummaryViewModel>().To<LapSummaryViewModel>();
            Bind<IReplayViewModel>().To<ReplayViewModel>();
            Bind<IPedalSectionViewModel>().To<PedalSectionViewModel>();
            Bind<IMapViewViewModel>().To<MapViewViewModel>();

            Bind<ILapCustomPathsCollection>().To<LapCustomPathsCollection>();
            Bind<IGraphViewModelsProvider>().To<GraphViewModelsProvider>();
            Bind<IGraphPanelController>().To<LeftGraphPanelController>();

            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
        }
    }
}