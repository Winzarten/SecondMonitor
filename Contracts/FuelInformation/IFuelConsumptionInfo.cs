namespace SecondMonitor.Contracts.FuelInformation
{
    using System;
    using DataModel.BasicProperties;

    public interface IFuelConsumptionInfo
    {
        Volume ConsumedFuel { get; }

        TimeSpan ElapsedTime { get; }

        Distance TraveledDistance { get; }

        IFuelConsumptionInfo AddConsumption(IFuelConsumptionInfo fuelConsumption);

        Volume GetAveragePerMinute();

        Volume GetAveragePerDistance(Distance distance);

        Volume GetAveragePerDistance(double distance);

    }
}