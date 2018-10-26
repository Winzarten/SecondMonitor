namespace SecondMonitor.ViewModels.CarStatus
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using WindowsControls.WPF.Commands;
    using DataModel.Snapshot;
    using Annotations;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using FuelStatus;

    public class CarStatusViewModel : ISimulatorDataSetViewModel, INotifyPropertyChanged
    {

        private readonly SimulatorDSViewModels _viewModels;

        private WaterTemperatureViewModel _waterTemperatureViewModel;
        private OilTemperatureViewModel _oilTemperatureViewModel;
        private CarWheelsViewModel _playersWheelsViewModel;
        private FuelOverviewViewModel _fuelOverviewViewModel;
        private FuelPlannerViewModel _fuelPlannerViewModel;
        private bool _isFuelCalculatorShown;

        private PedalsAndGearViewModel _pedalAndGearViewModel;

        public CarStatusViewModel()
        {
            _viewModels = new SimulatorDSViewModels { new OilTemperatureViewModel(), new WaterTemperatureViewModel(), new CarWheelsViewModel(), new FuelOverviewViewModel(), new PedalsAndGearViewModel()};
            RefreshProperties();
        }

        public OilTemperatureViewModel OilTemperatureViewModel
        {
            get => _oilTemperatureViewModel;
            private set
            {
                _oilTemperatureViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFuelCalculatorShown
        {
            get => _isFuelCalculatorShown;
            private set
            {
                _isFuelCalculatorShown = value;
                NotifyPropertyChanged();
            }
        }

        public FuelPlannerViewModel FuelPlannerViewModel
        {
            get => _fuelPlannerViewModel;
            set
            {
                _fuelPlannerViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public PedalsAndGearViewModel PedalsAndGearViewModel
        {
            get => _pedalAndGearViewModel;
            set
            {
                _pedalAndGearViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public WaterTemperatureViewModel WaterTemperatureViewModel
        {
            get => _waterTemperatureViewModel;
            private set
            {
                _waterTemperatureViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public CarWheelsViewModel PlayersWheelsViewModel
        {
            get => _playersWheelsViewModel;
            private set
            {
                _playersWheelsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public FuelOverviewViewModel FuelOverviewViewModel
        {
            get => _fuelOverviewViewModel;
            private set
            {
                _fuelOverviewViewModel = value;
                NotifyPropertyChanged();
            }
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

            FuelPlannerViewModel newViewModel = new FuelPlannerViewModel();        
            FuelOverviewViewModel.FuelConsumptionMonitor.SessionFuelConsumptionInfos.ForEach(x =>
            {
                SessionFuelConsumptionViewModel consumptionViewModel = new SessionFuelConsumptionViewModel();
                if (x != null)
                {
                    consumptionViewModel.FromModel(x);
                    newViewModel.Sessions.Add(consumptionViewModel);
                }
               
            });

            if (newViewModel.Sessions.Count == 0)
            {
                return;
            }
            newViewModel.SelectedSession = newViewModel.Sessions.First();
            FuelPlannerViewModel = newViewModel;
            IsFuelCalculatorShown = true;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}