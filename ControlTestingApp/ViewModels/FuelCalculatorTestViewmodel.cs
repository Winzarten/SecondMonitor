namespace ControlTestingApp.ViewModels
{
    using System;
    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.ViewModels.CarStatus.FuelStatus;

    public class FuelCalculatorTestViewModel : FuelCalculatorViewModel
    {
        public FuelCalculatorTestViewModel()
        {
            FuelConsumption = new FuelConsumptionInfo(Volume.FromLiters(20), TimeSpan.FromMinutes(30), 12000 );
            LapDistance = 3400;
            RequiredFuel = Volume.FromLiters(250);
            RequiredMinutes = 10;
            RequiredLaps = 30;
        }
    }
}