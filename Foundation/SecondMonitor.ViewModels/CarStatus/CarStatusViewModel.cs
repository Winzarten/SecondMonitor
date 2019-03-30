namespace SecondMonitor.ViewModels.CarStatus
{
    using System.Windows.Input;
    using Contracts.Commands;
    using DataModel.Snapshot;
    using FuelStatus;

    public class CarStatusViewModel : AbstractViewModel, ISimulatorDataSetViewModel
    {

        private readonly SimulatorDSViewModels _viewModels;
        private readonly FuelPlannerViewModelFactory _fuelPlannerViewModelFactory;

        private CarWheelsViewModel _playersWheelsViewModel;
        private FuelOverviewViewModel _fuelOverviewViewModel;
        private FuelPlannerViewModel _fuelPlannerViewModel;
        private CarSystemsViewModel _carSystemsViewModel;
        private DashboardViewModel _dashboardViewModel;
        private bool _isFuelCalculatorShown;

        private PedalsAndGearViewModel _pedalAndGearViewModel;

        public CarStatusViewModel(IPaceProvider paceProvider)
        {
            _viewModels = new SimulatorDSViewModels {new CarWheelsViewModel(), new FuelOverviewViewModel(paceProvider), new PedalsAndGearViewModel(), new CarSystemsViewModel(), new DashboardViewModel()};
            _fuelPlannerViewModelFactory = new FuelPlannerViewModelFactory();;
            RefreshProperties();
        }

        public bool IsFuelCalculatorShown
        {
            get => _isFuelCalculatorShown;
            private set => SetProperty(ref _isFuelCalculatorShown, value);
        }

        public FuelPlannerViewModel FuelPlannerViewModel
        {
            get => _fuelPlannerViewModel;
            set => SetProperty(ref _fuelPlannerViewModel, value);
        }

        public PedalsAndGearViewModel PedalsAndGearViewModel
        {
            get => _pedalAndGearViewModel;
            set => SetProperty(ref _pedalAndGearViewModel, value);
        }

        public CarWheelsViewModel PlayersWheelsViewModel
        {
            get => _playersWheelsViewModel;
            private set => SetProperty(ref _playersWheelsViewModel, value);
        }

        public FuelOverviewViewModel FuelOverviewViewModel
        {
            get => _fuelOverviewViewModel;
            private set => SetProperty(ref _fuelOverviewViewModel, value);
        }

        public CarSystemsViewModel CarSystemsViewModel
        {
            get => _carSystemsViewModel;
            private set => SetProperty(ref _carSystemsViewModel, value);
        }

        public DashboardViewModel DashboardViewModel
        {
            get => _dashboardViewModel;
            set => SetProperty(ref _dashboardViewModel, value);
        }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
           _viewModels.ApplyDateSet(dataSet);

        }

        public ICommand ShowFuelCalculatorCommand => new RelayCommand(ShowFuelCalculator);
        public ICommand HideFuelCalculatorCommand => new RelayCommand(HideFuelCalculator);

        public void Reset()
        {
            _viewModels.Reset();
        }

        private void ShowFuelCalculator()
        {
            if (FuelOverviewViewModel.FuelConsumptionMonitor.SessionFuelConsumptionInfos.Count == 0)
            {
                return;
            }

            FuelPlannerViewModel = _fuelPlannerViewModelFactory.Create(FuelOverviewViewModel);
            IsFuelCalculatorShown = FuelPlannerViewModel.Sessions.Count  != 0;
        }

        private void HideFuelCalculator()
        {
            IsFuelCalculatorShown = false;
        }

        private void RefreshProperties()
        {
            PlayersWheelsViewModel = _viewModels.GetFirst<CarWheelsViewModel>();
            FuelOverviewViewModel = _viewModels.GetFirst<FuelOverviewViewModel>();
            PedalsAndGearViewModel = _viewModels.GetFirst<PedalsAndGearViewModel>();
            CarSystemsViewModel = _viewModels.GetFirst<CarSystemsViewModel>();
            DashboardViewModel = _viewModels.GetFirst<DashboardViewModel>();
        }
    }
}