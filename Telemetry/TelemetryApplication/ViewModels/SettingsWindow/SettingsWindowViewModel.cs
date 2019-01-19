namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SettingsWindow
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using DataModel.Extensions;
    using Factory;
    using SecondMonitor.ViewModels;
    using Settings.DTO;

    public class SettingsWindowViewModel : AbstractViewModel<TelemetrySettingsDto>, ISettingsWindowViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private ICommand _openWindowCommand;
        private ICommand _cancelCommand;
        private ICommand _okCommand;
        private bool _isWindowOpened;
        private XAxisKind _xAxisKind;
        private ObservableCollection<IGraphSettingsViewModel> _leftPanelGraphs;
        private ObservableCollection<IGraphSettingsViewModel> _rightPanelGraphs;
        private ObservableCollection<IGraphSettingsViewModel> _notUsedGraphs;

        public SettingsWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _leftPanelGraphs = new ObservableCollection<IGraphSettingsViewModel>();
            _rightPanelGraphs = new ObservableCollection<IGraphSettingsViewModel>();
            _notUsedGraphs = new ObservableCollection<IGraphSettingsViewModel>();
        }

        public XAxisKind XAxisKind
        {
            get => _xAxisKind;
            set => SetProperty(ref _xAxisKind,value);
        }

        public bool IsWindowOpened
        {
            get => _isWindowOpened;
            set => SetProperty(ref _isWindowOpened, value);
        }

        public ObservableCollection<IGraphSettingsViewModel> LeftPanelGraphs
        {
            get => _leftPanelGraphs;
            private set => SetProperty(ref _leftPanelGraphs, value);
        }

        public ObservableCollection<IGraphSettingsViewModel> RightPanelGraphs
        {
            get => _rightPanelGraphs;
            private set => SetProperty(ref _rightPanelGraphs, value);
        }

        public ObservableCollection<IGraphSettingsViewModel> NotUsedGraphs
        {
            get => _notUsedGraphs;
            private set => SetProperty(ref _notUsedGraphs, value);
        }

        public ICommand OpenWindowCommand
        {
            get => _openWindowCommand;
            set => SetProperty(ref _openWindowCommand, value);
        }

        public ICommand CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }

        public ICommand OkCommand
        {
            get => _okCommand;
            set => SetProperty(ref _okCommand, value);
        }

        public override TelemetrySettingsDto SaveToNewModel()
        {
            LeftPanelGraphs.ForEach(x => x.GraphLocation = GraphLocationKind.LeftPanel);
            RightPanelGraphs.ForEach(x => x.GraphLocation = GraphLocationKind.RightPanel);
            NotUsedGraphs.ForEach(x => x.GraphLocation = GraphLocationKind.NoPanel);
            List<GraphSettingsDto> newGraphSettingsDto = LeftPanelGraphs.Concat(RightPanelGraphs.Concat(NotUsedGraphs)).Select(x => x.SaveToNewModel()).ToList();
            for (int i = 0; i < newGraphSettingsDto.Count; i++)
            {
                newGraphSettingsDto[i].GraphPriority = i;
            }

            return new TelemetrySettingsDto()
            {
                GraphSettings = newGraphSettingsDto,
                XAxisKind = XAxisKind
            };
        }

        protected override void ApplyModel(TelemetrySettingsDto model)
        {
            ObservableCollection<IGraphSettingsViewModel> newLeftPanelGraphs = new ObservableCollection<IGraphSettingsViewModel>();
            ObservableCollection<IGraphSettingsViewModel> newRightPanelGraphs = new ObservableCollection<IGraphSettingsViewModel>();
            ObservableCollection<IGraphSettingsViewModel> newNotUsedPanelGraphs = new ObservableCollection<IGraphSettingsViewModel>();
            foreach (GraphSettingsDto modelGraphSetting in model.GraphSettings)
            {
                IGraphSettingsViewModel newViewModel = _viewModelFactory.Create<IGraphSettingsViewModel>();
                newViewModel.FromModel(modelGraphSetting);
                switch (modelGraphSetting.GraphLocation)
                {
                    case GraphLocationKind.LeftPanel:
                        newLeftPanelGraphs.Add(newViewModel);
                        break;
                    case GraphLocationKind.RightPanel:
                        newRightPanelGraphs.Add(newViewModel);
                        break;
                    case GraphLocationKind.NoPanel:
                        newNotUsedPanelGraphs.Add(newViewModel);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            LeftPanelGraphs = newLeftPanelGraphs;
            RightPanelGraphs = newRightPanelGraphs;
            NotUsedGraphs = newNotUsedPanelGraphs;
            XAxisKind = model.XAxisKind;
        }
    }
}