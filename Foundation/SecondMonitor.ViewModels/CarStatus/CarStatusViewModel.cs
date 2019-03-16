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

        private WaterTemperatureViewModel _waterTemperatureViewModel;
        private OilTemperatureViewModel _oilTemperatureViewModel;
        private CarWheelsViewModel _playersWheelsViewModel;
        private FuelOverviewViewModel _fuelOverviewViewModel;
        private FuelPlannerViewModel _fuelPlannerViewModel;
        private CarSystemsViewModel _carSystemsViewModel;
        private bool _isFuelCalculatorShown;

        private PedalsAndGearViewModel _pedalAndGearViewModel;

        public CarStatusViewModel(IPaceProvider paceProvider)
        {
            _viewModels = new SimulatorDSViewModels { new OilTemperatureViewModel(), new WaterTemperatureViewModel(), new CarWheelsViewModel(), new FuelOverviewViewModel(paceProvider), new PedalsAndGearViewModel(), new CarSystemsViewModel()};
            _fuelPlannerViewModelFactory = new FuelPlannerViewModelFactory();;
            RefreshProperties();
        }

        public OilTemperatureViewModel OilTemperatureViewModel
        {
            get => _oilTemperatureViewModel;
            private set => SetProperty(ref _oilTemperatureViewModel, value);
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

        public WaterTemperatureViewModel WaterTemperatureViewModel
        {
            get => _waterTemperatureViewModel;
            private set => SetProperty(ref _waterTemperatureViewModel, value);
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
            OilTemperatureViewModel = _viewModels.GetFirst<OilTemperatureViewModel>();
            WaterTemperatureViewModel = _viewModels.GetFirst<WaterTemperatureViewModel>();
            PlayersWheelsViewModel = _viewModels.GetFirst<CarWheelsViewModel>();
            FuelOverviewViewModel = _viewModels.GetFirst<FuelOverviewViewModel>();
            PedalsAndGearViewModel = _viewModels.GetFirst<PedalsAndGearViewModel>();
            CarSystemsViewModel = _viewModels.GetFirst<CarSystemsViewModel>();

        }
    }
}