namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels
{
    using System.Collections.Generic;
    using DataModel.Extensions;
    using Factory;
    using GraphPanel;
    using LapPicker;
    using MapView;
    using SecondMonitor.ViewModels;
    using SnapshotSection;

    public class MainWindowViewModel : AbstractViewModel, IMainWindowViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private bool _isBusy;
        private readonly List<IGraphViewModel> _leftPanelGraphs;
        private readonly List<IGraphViewModel> _rightPanelGraphs;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _leftPanelGraphs = new List<IGraphViewModel>();
            _rightPanelGraphs = new List<IGraphViewModel>();
            LapSelectionViewModel = viewModelFactory.Create<ILapSelectionViewModel>();
            SnapshotSectionViewModel = viewModelFactory.Create<ISnapshotSectionViewModel>();
            MapViewViewModel = viewModelFactory.Create<IMapViewViewModel>();
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }
        public ILapSelectionViewModel LapSelectionViewModel { get; }
        public ISnapshotSectionViewModel SnapshotSectionViewModel { get; }
        public IMapViewViewModel MapViewViewModel { get; }

        public IReadOnlyCollection<IGraphViewModel> LeftPanelGraphs => _leftPanelGraphs.AsReadOnly();
        public IReadOnlyCollection<IGraphViewModel> RightPanelGraphs => _rightPanelGraphs.AsReadOnly();

        public void ClearLeftPanelGraphs()
        {
            _leftPanelGraphs.Clear();
            NotifyPropertyChanged(nameof(LeftPanelGraphs));
        }

        public void AddToLeftPanelGraphs(params IGraphViewModel[] graphViewModels)
        {
            graphViewModels.ForEach(_leftPanelGraphs.Add);
            NotifyPropertyChanged(nameof(LeftPanelGraphs));
        }

        public void ClearRightPanelGraphs()
        {
            _rightPanelGraphs.Clear();
            NotifyPropertyChanged(nameof(RightPanelGraphs));
        }

        public void AddToRightPanelGraphs(params IGraphViewModel[] graphViewModels)
        {
            graphViewModels.ForEach(_rightPanelGraphs.Add);
            NotifyPropertyChanged(nameof(RightPanelGraphs));
        }
    }
}