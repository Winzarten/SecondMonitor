using System.ComponentModel;

namespace SecondMonitor.DataModel.BasicProperties.FuelConsumption
{
    using System;

    public class FuelPerDistance : IQuantity
    {
        public FuelPerDistance(Volume consumedFuel, Distance distance)
        {
            ConsumedFuel = Volume.FromLiters(consumedFuel.InLiters);
            Distance = Distance.FromMeters(distance.InMeters);
        }

        public Volume ConsumedFuel { get; }
        public Distance Distance { get; }

        public IQuantity ZeroQuantity => new FuelPerDistance(Volume.FromLiters(0), Distance.ZeroDistance);
        public bool IsZero => Distance.IsZero;
        public double RawValue => InVolumePer100Km.InLiters;

        public Volume InVolumePer100Km
        {
            get
            {
                double normalizedDistance = Distance.InKilometers / 100;
                return Volume.FromLiters(ConsumedFuel.InLiters / normalizedDistance);
            }
        }

        public Distance InDistancePerGallon => Distance.FromMeters(Distance.InMeters / ConsumedFuel.InUsGallons);

        public double GetConsumption(FuelPerDistanceUnits fuelPerDistanceUnits)
        {
            switch (fuelPerDistanceUnits)
            {
                case FuelPerDistanceUnits.LitersPerHundredKm:
                    return InVolumePer100Km.InLiters;
                case FuelPerDistanceUnits.MilesPerGallon:
                    return InDistancePerGallon.InMiles;
                default:
                    throw new InvalidEnumArgumentException($"Unknown fuel consumption unit: {fuelPerDistanceUnits}");
            }
        }

        public static string GetUnitsSymbol(FuelPerDistanceUnits fuelPerDistanceUnits)
        {
            switch (fuelPerDistanceUnits)
            {
                case FuelPerDistanceUnits.LitersPerHundredKm:
                    return "l/100km";
                case FuelPerDistanceUnits.MilesPerGallon:
                    return "mpg";
                default:
                    throw new ArgumentException($"Unknown units {fuelPerDistanceUnits}");
            }
        }
    }
}
