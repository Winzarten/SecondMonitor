using System.ComponentModel;

namespace SecondMonitor.DataModel.BasicProperties.FuelConsumption
{
    public class FuelPerDistance
    {
        public FuelPerDistance(Volume consumedFuel, Distance distance)
        {
            ConsumedFuel = consumedFuel;
            Distance = distance;
        }

        public Volume ConsumedFuel { get; }
        public Distance Distance { get; }

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
    }
}
