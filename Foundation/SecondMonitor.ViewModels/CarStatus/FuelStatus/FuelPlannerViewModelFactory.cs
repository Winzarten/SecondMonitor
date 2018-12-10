using System.Linq;
using SecondMonitor.DataModel.Extensions;

namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    public class FuelPlannerViewModelFactory
    {
        public FuelPlannerViewModel Create(FuelOverviewViewModel fuelOverviewViewModel)
        {
            FuelPlannerViewModel newViewModel =  new FuelPlannerViewModel();
            fuelOverviewViewModel.FuelConsumptionMonitor.SessionFuelConsumptionInfos.ForEach(x =>
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
                return newViewModel;
            }

            newViewModel.SelectedSession = newViewModel.Sessions.First();
            return newViewModel;
        }
    }
}