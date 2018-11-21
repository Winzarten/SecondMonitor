namespace SecondMonitor.Contracts.FuelInformation
{
    using DataModel.BasicProperties;

    public interface ISessionFuelConsumptionViewModel
    {
         string TrackName { get; set; }
         Distance LapDistance { get; set; }
         string SessionType { get; set; }
         IFuelConsumptionInfo FuelConsumption { get; set; }
         Volume AvgPerMinute { get; set; }
         Volume AvgPerLap { get; set; }

    }
}