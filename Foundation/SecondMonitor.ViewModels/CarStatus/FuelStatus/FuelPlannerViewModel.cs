namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System.Collections.ObjectModel;
    using Contracts.FuelInformation;

    public class FuelPlannerViewModel : AbstractViewModel, IFuelPlannerViewModel
    {
        private ISessionFuelConsumptionViewModel _selectedSession;
        private IFuelCalculatorViewModel _calculatorForSelectedSession;

        public FuelPlannerViewModel()
        {
            Sessions = new ObservableCollection<ISessionFuelConsumptionViewModel>();
        }

        public ObservableCollection<ISessionFuelConsumptionViewModel> Sessions { get; }
        public ISessionFuelConsumptionViewModel SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                UpdateCalculatorViewModel();
                NotifyPropertyChanged();
            }
        }

        public IFuelCalculatorViewModel CalculatorForSelectedSession
        {
            get => _calculatorForSelectedSession;
            set
            {
                _calculatorForSelectedSession = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateCalculatorViewModel()
        {
            CalculatorForSelectedSession = new FuelCalculatorViewModel()
            {
                FuelConsumption = SelectedSession.FuelConsumption,
                LapDistance = SelectedSession.LapDistance.InMeters,
            };
        }
    }
}