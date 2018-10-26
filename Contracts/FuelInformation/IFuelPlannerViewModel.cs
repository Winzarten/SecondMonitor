namespace SecondMonitor.Contracts.FuelInformation
{
    using System.Collections.ObjectModel;

    public interface IFuelPlannerViewModel
    {
        ObservableCollection<ISessionFuelConsumptionViewModel> Sessions { get; }
        ISessionFuelConsumptionViewModel SelectedSession { get; set; }
        IFuelCalculatorViewModel CalculatorForSelectedSession { get; set; }
    }
}