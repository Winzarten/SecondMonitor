namespace SecondMonitor.Telemetry.TelemetryApplication
{
    using WindowsControls.WPF.UserInput;
    using AggregatedCharts;
    using AggregatedCharts.Filter;
    using AggregatedCharts.Histogram;
    using AggregatedCharts.Histogram.Extractors;
    using AggregatedCharts.Histogram.Providers;
    using AggregatedCharts.ScatterPlot.Extractors;
    using AggregatedCharts.ScatterPlot.Providers;
    using Contracts.UserInput;
    using Controllers.AggregatedChart;
    using Controllers.MainWindow;
    using Controllers.MainWindow.GraphPanel;
    using Controllers.MainWindow.LapPicker;
    using Controllers.MainWindow.MapView;
    using Controllers.MainWindow.Replay;
    using Controllers.MainWindow.Snapshot;
    using Controllers.OpenWindow;
    using Controllers.SettingsWindow;
    using Controllers.Synchronization;
    using Controllers.Synchronization.Graphs;
    using Controllers.TelemetryLoad;
    using LapAutoSelector;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;
    using Repository;
    using SecondMonitor.ViewModels.Colors;
    using Settings;
    using Settings.DTO;
    using SimdataManagement;
    using TelemetryManagement.StoryBoard;
    using ViewModels;
    using ViewModels.AggregatedCharts;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.Chassis;
    using ViewModels.GraphPanel.DataExtractor;
    using ViewModels.GraphPanel.Inputs;
    using ViewModels.GraphPanel.Wheels;
    using ViewModels.LapPicker;
    using ViewModels.LoadedLapCache;
    using ViewModels.MapView;
    using ViewModels.OpenWindow;
    using ViewModels.Replay;
    using ViewModels.SettingsWindow;
    using ViewModels.SnapshotSection;

    public class TelemetryApplicationModule : NinjectModule
    {

        private const string MainWidowScopeName = "MainWindow";

        public override void Load()
        {
            Bind<IMainWindowController>().To<MainWindowController>().DefinesNamedScope(MainWidowScopeName);
            Bind<TelemetryStoryBoardFactory>().ToSelf();
            Bind<IMapsLoaderFactory>().To<MapsLoaderFactory>();
            Bind<ILoadedLapsCache>().To<LoadedLapsCache>().InSingletonScope();
            Bind<ITelemetryLoadController>().To<TelemetryLoadController>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetryViewsSynchronization>().To<TelemetryViewsSynchronization>().InSingletonScope();
            Bind<IGraphViewSynchronization>().To<GraphViewSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IDataPointSelectionSynchronization>().To<DataPointSelectionSynchronization>().InSingletonScope();
            Bind<IMainWindowViewModel>().To<MainWindowViewModel>().InNamedScope(MainWidowScopeName);
            Bind<IMapViewController>().To<MapViewController>().InNamedScope(MainWidowScopeName);
            Bind<ILapColorSynchronization>().To<LapColorSynchronization>().InNamedScope(MainWidowScopeName);
            Bind<IColorPaletteProvider>().To<BasicColorPaletteProvider>().InNamedScope(MainWidowScopeName);
            Bind<ITelemetrySettingsRepository>().To<TelemetrySettingsRepository>().InNamedScope(MainWidowScopeName);
            Bind<IReplayController>().To<ReplayController>();
            Bind<IOpenWindowController>().To<OpenWindowController>();
            Bind<ISnapshotSectionController>().To<SnapshotSectionController>();
            Bind<IGraphsSettingsProvider>().To<StoredGraphsSettingsProvider>().InNamedScope(MainWidowScopeName); ;
            Bind<IGraphsSettingsProvider>().To<DefaultGraphsSettingsProvider>().WhenInjectedExactlyInto<StoredGraphsSettingsProvider>();
            Bind<ISettingsWindowController>().To<SettingsWindowController>();
            Bind<IAggregatedChartsController>().To<AggregatedChartsController>();

            Bind<ITelemetryRepositoryFactory>().To<TelemetryRepositoryFactory>();
            Bind<ILapPickerController>().To<LapPickerController>();
            Bind<ISnapshotSectionViewModel>().To<SnapshotSectionViewModel>();

            Bind<ILapSelectionViewModel>().To<LapSelectionViewModel>();
            Bind<ILapSummaryViewModel>().To<LapSummaryViewModel>();
            Bind<IReplayViewModel>().To<ReplayViewModel>();
            Bind<IPedalSectionViewModel>().To<PedalSectionViewModel>();
            Bind<IMapViewViewModel>().To<MapViewViewModel>();
            Bind<IOpenWindowViewModel>().To<OpenWindowViewModel>();
            Bind<IOpenWindowSessionInformationViewModel>().To<OpenWindowSessionInformationViewModel>();
            Bind<ISettingsWindowViewModel>().To<SettingsWindowViewModel>();
            Bind<IGraphSettingsViewModel>().To<GraphSettingsViewModel>();

            Bind<ILapCustomPathsCollection>().To<LapCustomPathsCollection>();
            Bind<IGraphViewModelsProvider>().To<GraphViewModelsProvider>();
            Bind<IGraphPanelController>().To<LeftGraphPanelController>();
            Bind<IGraphPanelController>().To<RightGraphPanelController>();
            Bind<IUserInputProvider>().To<DialogUserInputProvider>();

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
            Bind<IGraphViewModel>().To<BrakeTemperaturesGraphViewModel>();
            Bind<IGraphViewModel>().To<TyrePressuresGraphViewModel>();
            Bind<IGraphViewModel>().To<LeftFrontTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<RightFrontTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<LeftRearTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<RightRearTyreTempsGraphViewModel>();
            Bind<IGraphViewModel>().To<WheelRpsGraphViewModel>();
            Bind<IGraphViewModel>().To<SuspensionTravelGraphViewModel>();
            Bind<IGraphViewModel>().To<RideHeightGraphViewModel >();
            Bind<IGraphViewModel>().To<ChassisRideHeightGraphViewModel>();
            Bind<IGraphViewModel>().To<SuspensionVelocityGraphViewModel>();
            Bind<IGraphViewModel>().To<OverallDownForceGraphViewModel>();
            Bind<IGraphViewModel>().To<TurboBoostGraphViewModel>();
            Bind<IGraphViewModel>().To<FrontRearDownForceGraphViewModel>();
            Bind<IGraphViewModel>().To<CamberGraphViewModel>();
            Bind<IGraphViewModel>().To<RakeGraphViewModel>();

            Bind<ISingleSeriesDataExtractor>().To<SimpleSingleSeriesDataExtractor>();
            Bind<ISingleSeriesDataExtractor>().To<CompareToReferenceDataExtractor>();

            Bind<ILapAutoSelector>().To<EmptyLapAutoSelector>();

            Bind<IAggregatedChartProvider>().To<SuspensionVelocityHistogramProvider>();
            Bind<IAggregatedChartProvider>().To<SpeedToRpmChartProvider>();
            Bind<IAggregatedChartProvider>().To<SpeedHorizontalAccelerationChartProvider>();
            Bind<IAggregatedChartProvider>().To<RpmToHorizontalGChartProvider>();
            Bind<IAggregatedChartProvider>().To<RpmHistogramProvider>();
            Bind<IAggregatedChartProvider>().To<RideHeightHistogramProvider>();
            Bind<IAggregatedChartProvider>().To<RideHeightToHorizontalAccProvider>();
            Bind<IAggregatedChartProvider>().To<RideHeightToLateralAccProvider>();
            Bind<IAggregatedChartProvider>().To<RideHeightToSpeedProvider>();
            Bind<IAggregatedChartProvider>().To<SpeedToDownforceProvider>();
            Bind<IAggregatedChartProvider>().To<SpeedToRakeProvider>();
            Bind<IAggregatedChartProvider>().To<LatToLogGProvider>();
            Bind<IAggregatedChartProvider>().To<RearRollAngleToFrontRollAngleProvider>();

            Bind<SuspensionVelocityHistogramDataExtractor>().ToSelf();
            Bind<RideHeightGraphViewModel>().ToSelf();

            Bind<SpeedToRpmScatterPlotExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<NoClutchFilter>().WhenInjectedExactlyInto<SpeedToRpmScatterPlotExtractor>();
            Bind<ITelemetryFilter>().To<NoBrakeFilter>().WhenInjectedExactlyInto<SpeedToRpmScatterPlotExtractor>();

            Bind<HorizontalAccelerationToRideHeightExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<NoLateralAccelerationFilter>().WhenInjectedExactlyInto<HorizontalAccelerationToRideHeightExtractor>();

            Bind<LateralAccelerationToRideHeightExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<NoHorizontalAccelerationFilter>().WhenInjectedExactlyInto<LateralAccelerationToRideHeightExtractor>();

            Bind<SpeedToRakeExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<NoLateralAccelerationFilter>().WhenInjectedExactlyInto<SpeedToRakeExtractor>();

            Bind<SpeedToRideHeightExtractor>().ToSelf();
            Bind<SpeedToDownforceExtractor>().ToSelf();

            Bind<SpeedToHorizontalGExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<FullThrottleFilter>().WhenInjectedExactlyInto<SpeedToHorizontalGExtractor>();
            Bind<ITelemetryFilter>().To<NoBrakeFilter>().WhenInjectedExactlyInto<SpeedToHorizontalGExtractor>();
            Bind<ITelemetryFilter>().To<NoClutchFilter>().WhenInjectedExactlyInto<SpeedToHorizontalGExtractor>();

            Bind<RpmToHorizontalGExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<FullThrottleFilter>().WhenInjectedExactlyInto<RpmToHorizontalGExtractor>();
            Bind<ITelemetryFilter>().To<NoBrakeFilter>().WhenInjectedExactlyInto<RpmToHorizontalGExtractor>();
            Bind<ITelemetryFilter>().To<NoClutchFilter>().WhenInjectedExactlyInto<RpmToHorizontalGExtractor>();

            Bind<RpmHistogramDataExtractor>().ToSelf();
            Bind<ITelemetryFilter>().To<NoBrakeFilter>().WhenInjectedExactlyInto<RpmHistogramDataExtractor>();
            Bind<ITelemetryFilter>().To<NoClutchFilter>().WhenInjectedExactlyInto<RpmHistogramDataExtractor>();

            Bind<LateralToLongGExtractor>().ToSelf();
            Bind<RearRollAngleToFrontRollAngleExtractor>().ToSelf();
            Bind<LateralAccFilter>().ToSelf();
            Bind<ThrottlePositionFilter>().ToSelf();

            Bind<IAggregatedChartSelectorViewModel>().To<AggregatedChartSelectorViewModel>();
            Bind<IGearTelemetryFilter>().To<GearTelemetryFilter>();
        }
    }
}