namespace SecondMonitor.WindowsControls.WPF.CarSettingsControl
{
    using DataModel.BasicProperties;

    public interface ITyreSettingsViewModel
    {
        string CompoundName { get; set; }
        bool IsGlobalCompound { get; }

        Temperature IdealTyreTemperature { get; set; }
        Temperature IdealTyreTemperatureWindow { get; set; }

        Pressure IdealTyrePressure { get; set; }
        Pressure IdealTyrePressureWindow { get; set; }
    }
}