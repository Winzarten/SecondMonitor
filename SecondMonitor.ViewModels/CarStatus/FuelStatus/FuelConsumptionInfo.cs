
namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System;
    using DataModel.BasicProperties.FuelConsumption;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    public class FuelConsumptionInfo
    {
        public FuelConsumptionInfo()
        {
            ConsumedFuel = Volume.FromLiters(0);
            ElapsedTime = TimeSpan.Zero;
            TraveledDistance = Distance.ZeroDistance;
        }

        public FuelConsumptionInfo(Volume consumedFuel, TimeSpan elapsedTime, double traveledDistance)
        :this(consumedFuel, elapsedTime, Distance.FromMeters(traveledDistance))
        {
         
        }

        public FuelConsumptionInfo(Volume consumedFuel, TimeSpan elapsedTime, Distance traveledDistance)
        {
            ConsumedFuel = consumedFuel;
            ElapsedTime = elapsedTime;
            TraveledDistance = traveledDistance;
        }

        public Volume ConsumedFuel { get; }

        public TimeSpan ElapsedTime { get; }

        public Distance TraveledDistance { get; }

        public FuelPerDistance Consumption => new FuelPerDistance(ConsumedFuel, Distance.FromMeters(TraveledDistance.InMeters));

        public static FuelConsumptionInfo CreateConsumption(FuelStatusSnapshot fromSnapshot, FuelStatusSnapshot toSnapshot)
        {
            return new FuelConsumptionInfo(fromSnapshot.FuelLevel - toSnapshot.FuelLevel, toSnapshot.SessionTime - fromSnapshot.SessionTime, toSnapshot.TotalDistance - fromSnapshot.TotalDistance);
        }

        public FuelConsumptionInfo AddConsumption(FuelConsumptionInfo fuelConsumption)
        {
            TimeSpan newElapsedTime = ElapsedTime + fuelConsumption.ElapsedTime;
            return new FuelConsumptionInfo(ConsumedFuel + fuelConsumption.ConsumedFuel, newElapsedTime, TraveledDistance.InMeters + fuelConsumption.TraveledDistance.InMeters);
        }

        public Volume GetAveragePerMinute()
        {
            return Volume.FromLiters(ConsumedFuel.InLiters / ElapsedTime.TotalMinutes);
        }

        public Volume GetAveragePerDistance(Distance distance) => GetAveragePerDistance(distance.InMeters);

        public Volume GetAveragePerDistance(double distance)
        {
            double distanceCoef = TraveledDistance.InMeters / distance;
            return Volume.FromLiters(ConsumedFuel.InLiters / distanceCoef);
        }

        public bool IsFuelConsumptionValid(SimulatorDataSet dataSet)
        {
            if (TraveledDistance.InMeters < 0)
            {
                return false;
            }

            if (TraveledDistance > dataSet.SessionInfo.TrackInfo.LayoutLength * 0.8)
            {
                return false;
            }

            if (ElapsedTime < TimeSpan.Zero)
            {
                return false;
            }

            if (ConsumedFuel.InLiters < 0)
            {
                return false;
            }

            // Consumed 0.5l while the car is stationary - doesn't add up. Most likely the car placed i.e at Assetto Corsa time trial start
            if (dataSet.PlayerInfo.Speed.InKph < 1 && ConsumedFuel.InLiters > 0.5)
            {
                return false;
            }

            return true;
        }
    }
}