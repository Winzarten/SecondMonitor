namespace ControlTestingApp.ViewModels
{
    using System;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.ViewModels.CarStatus.FuelStatus;

    public class FuelCalculatorTestViewModel
    {
        public FuelCalculatorTestViewModel()
        {
            FuelCalculatorViewModel = new FuelCalculatorViewModel()
            {
                FuelConsumption = new FuelConsumptionInfo(Volume.FromLiters(20), TimeSpan.FromMinutes(30), 12000),
                LapDistance = 3400,
                RequiredFuel = Volume.FromLiters(250),
                RequiredMinutes = 10,
                RequiredLaps = 30,
            };

            SessionFuelConsumptionViewModel = new SessionFuelConsumptionViewModel()
            {
                FuelConsumption = FuelCalculatorViewModel.FuelConsumption,
                LapDistance = Distance.FromMeters(3400),
                SessionType = SessionType.Qualification.ToString(),
                TrackName = "Brands Hatch",
            };
        }

        public FuelCalculatorViewModel FuelCalculatorViewModel { get; set; }
        public SessionFuelConsumptionViewModel SessionFuelConsumptionViewModel { get; set; }
    }
}