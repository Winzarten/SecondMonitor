using System.ComponentModel;
using System.Runtime.CompilerServices;
using SecondMonitor.DataModel;
using SecondMonitor.Timing.Annotations;

namespace SecondMonitor.Timing.Model.Settings.Model
{
    public class DisplaySettings
    {
        public TemperatureUnits TemperatureUnits { get; set; } = TemperatureUnits.Celsius;

        public PressureUnits PressureUnits { get; set; } = PressureUnits.Kpa;

        public VolumeUnits VolumeUnits { get; set; } = VolumeUnits.Liters;
    }
}
