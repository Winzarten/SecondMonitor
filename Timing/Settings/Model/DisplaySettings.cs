namespace SecondMonitor.Timing.Settings.Model
{
    using System;

    using SecondMonitor.DataModel.BasicProperties;

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

        public ReportingSettings ReportingSettings { get; set; } = new ReportingSettings();

        public DisplaySettings()
        {
            RaceOptions.ColumnsSettings.Sector1.Visible = false;
            RaceOptions.ColumnsSettings.Sector2.Visible = false;
            RaceOptions.ColumnsSettings.Sector3.Visible = false;
        }
    }

}
