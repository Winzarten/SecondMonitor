namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using System.Collections.Generic;
    using GraphPanel;
    using LapPicker;
    using MapView;
    using SecondMonitor.ViewModels;
    using SnapshotSection;

    public interface IMainWindowViewModel : IViewModel
    {
        bool IsBusy { get; set; }
        ILapSelectionViewModel LapSelectionViewModel { get; }
        ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
        IMapViewViewModel MapViewViewModel { get; }

        IReadOnlyCollection<IGraphViewModel> LeftPanelGraphs { get; }
        IReadOnlyCollection<IGraphViewModel> RightPanelGraphs { get;}

        void ClearLeftPanelGraphs();
        void AddToLeftPanelGraphs(params IGraphViewModel[] graphViewModels);

        void ClearRightPanelGraphs();
        void AddToRightPanelGraphs(params IGraphViewModel[] graphViewModels);
    }
}