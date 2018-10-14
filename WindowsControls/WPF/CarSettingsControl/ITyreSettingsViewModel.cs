namespace SecondMonitor.WindowsControls.WPF.CarSettingsControl
{
    using DataModel.BasicProperties;

    public interface ITyreSettingsViewModel
    {
        string CompoundName { get; set; }
        bool IsGlobalCompound { get; }

        Temperature MinimalIdealTyreTemperature { get; set; }
        Temperature MaximumIdealTyreTemperature { get; set; }

        Pressure MinimalIdealTyrePressure { get; set; }
        Pressure MaximumIdealTyrePressure { get; set; }
    }
}