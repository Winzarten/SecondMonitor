namespace ControlTestingApp.ViewModels
{
    using System;
    using System.Linq;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.ViewModels.CarStatus.FuelStatus;

    public class FuelCalculatorTestViewModel : FuelPlannerViewModel
    {
        public FuelCalculatorTestViewModel()
        {
            Sessions.Add(new SessionFuelConsumptionViewModel()
            {
                FuelConsumption = new FuelConsumptionInfo(Volume.FromLiters(20), TimeSpan.FromMinutes(30), 12000),
                LapDistance = Distance.FromMeters(3400),
                SessionType = SessionType.Qualification.ToString(),
                TrackName = "Brands Hatch",
            });

            Sessions.Add(new SessionFuelConsumptionViewModel()
            {
                FuelConsumption = new FuelConsumptionInfo(Volume.FromLiters(30), TimeSpan.FromMinutes(40), 25000),
                LapDistance = Distance.FromMeters(5200),
                SessionType = SessionType.Race.ToString(),
                TrackName = "Slovakia Ring",
            });
            SelectedSession = Sessions.First();
        }
    }
}