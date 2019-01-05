namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using Controllers.MainWindow;
    using Controllers.MainWindow.GraphPanel;
    using Controllers.MainWindow.LapPicker;
    using Controllers.MainWindow.MapView;
    using Controllers.MainWindow.Replay;
    using Controllers.MainWindow.Snapshot;
    using Controllers.Synchronization;
    using Controllers.Synchronization.Graphs;
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
    using ViewModels.GraphPanel.Inputs;
    using ViewModels.GraphPanel.Wheels;
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
            Bind<IGraphViewSynchronization>().To<GraphViewSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IMainWindowViewModel>().To<MainWindowViewModel>().InNamedScope(MainWidowScopeName);
            Bind<IMapViewController>().To<MapViewController>().InNamedScope(MainWidowScopeName);
            Bind<ILapColorSynchronization>().To<LapColorSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IColorPaletteProvider>().To<BasicColorPaletteProvider>().InNamedScope(MainWidowScopeName);
            Bind<IReplayController>().To<ReplayController>();
            Bind<ISnapshotSectionController>().To<SnapshotSectionController>();
            Bind<IGraphsSettingsProvider>().To<DefaultGraphsSettingsProvider>();

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
            Bind<IGraphPanelController>().To<RightGraphPanelController>();

            Bind<IGraphViewModel>().To<LapTimeGraphViewModel>();
            Bind<IGraphViewModel>().To<SteeringAngleGraphViewModel>();
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
            Bind<IGraphViewModel>().To<BrakeGraphViewModel>();
            Bind<IGraphViewModel>().To<ClutchGraphViewModel>();
            Bind<IGraphViewModel>().To<SpeedGraphViewModel>();
            Bind<IGraphViewModel>().To<EngineRpmGraphViewModel>();
            Bind<IGraphViewModel>().To<GearGraphViewModel>();
            Bind<IGraphViewModel>().To<LateralGGraphViewModel>();
            Bind<IGraphViewModel>().To<HorizontalGGraphViewModel>();
            /*Bind<IGraphViewModel>().To<BrakeTemperaturesGraphViewModel>();
            Bind<IGraphViewModel>().To<TyrePressuresGraphViewModel>();
            Bind<IGraphViewModel>().To<LeftFrontTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<RightFrontTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<LeftRearTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<RightRearTyreTempsGraphViewModel>();*/
            Bind<IGraphViewModel>().To<WheelRpsGraphViewModel>();
            Bind<IGraphViewModel>().To<SuspensionTravelGraphViewModel>();
            Bind<IGraphViewModel>().To<RideHeightGraphViewModel >();
            /*Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();
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
            Bind<IGraphViewModel>().To<ThrottleGraphViewModel>();*/
        }
    }
}