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

        public VelocityUnits VelocityUnits { get; set; } = VelocityUnits.Kph;

        public FuelCalculationScope FuelCalculationScope { get; set; } = FuelCalculationScope.Lap;

        public int PaceLaps { get; set; } = 3;

        public int RefreshRate { get; set; } = 300;

        public bool ScrollToPlayer { get; set; } = true;

        public SessionOptions PracticeOptions { get; set; } = new SessionOptions { OrderingDisplayMode = DisplayModeEnum.Absolute, TimesDisplayMode = DisplayModeEnum.Absolute, SessionName = "Practice" };

        public SessionOptions QualificationOptions { get; set; } = new SessionOptions { OrderingDisplayMode = DisplayModeEnum.Absolute, TimesDisplayMode = DisplayModeEnum.Absolute, SessionName = "Quali" };

        public SessionOptions RaceOptions { get; set; } = new SessionOptions { OrderingDisplayMode = DisplayModeEnum.Relative, TimesDisplayMode = DisplayModeEnum.Relative, SessionName = "Race" };
    }
}
