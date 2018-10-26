namespace SecondMonitor.Contracts.FuelInformation
{
    using DataModel.BasicProperties;

    public interface IFuelCalculatorViewModel
    {
        IFuelConsumptionInfo FuelConsumption { get; }
        int RequiredLaps { get; }
        int RequiredMinutes { get; }
        double LapDistance { get; }
        Volume RequiredFuel { get; }
    }
}
