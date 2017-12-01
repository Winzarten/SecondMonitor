using System;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.Timing.Model.Settings.Model
{
    [Serializable]
    public class DisplaySettings
    {
        public TemperatureUnits TemperatureUnits { get; set; } = TemperatureUnits.Celsius;

        public PressureUnits PressureUnits { get; set; } = PressureUnits.Kpa;

        public VolumeUnits VolumeUnits { get; set; } = VolumeUnits.Liters;

        public FuelCalculationScope FuelCalculationScope { get; set; } = FuelCalculationScope.Lap;
    }
}
