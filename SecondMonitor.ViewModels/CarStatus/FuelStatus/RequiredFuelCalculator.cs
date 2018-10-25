using System;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    public class RequiredFuelCalculator
    {
        private readonly FuelConsumptionInfo _fuelConsumptionInfo;

        public RequiredFuelCalculator(FuelConsumptionInfo fuelConsumptionInfo)
        {
            _fuelConsumptionInfo = fuelConsumptionInfo;
        }

        public Volume GetRequiredFuel(TimeSpan time)
        {
            return _fuelConsumptionInfo.GetAveragePerMinute() * time.TotalMinutes;
        }

        public Volume GetRequiredFuel(Distance distance)
        {
            return _fuelConsumptionInfo.GetAveragePerDistance(distance.InMeters);
        }

        public Volume GetRequiredFuel(TimeSpan time, Distance distance)
        {
            return Volume.FromLiters(GetRequiredFuel(time).InLiters + GetRequiredFuel(distance).InLiters);
        }
    }
}
