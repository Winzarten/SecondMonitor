namespace SecondMonitor.Contracts.FuelInformation
{
    using System;
    using DataModel.BasicProperties;
    using DataModel.BasicProperties.FuelConsumption;

    public interface IFuelConsumptionInfo
    {
        Volume ConsumedFuel { get; }

        TimeSpan ElapsedTime { get; }

        Distance TraveledDistance { get; }

        FuelPerDistance Consumption { get; }

        IFuelConsumptionInfo AddConsumption(IFuelConsumptionInfo fuelConsumption);

        Volume GetAveragePerMinute();

        Volume GetAveragePerDistance(Distance distance);

        Volume GetAveragePerDistance(double distance);

    }
}