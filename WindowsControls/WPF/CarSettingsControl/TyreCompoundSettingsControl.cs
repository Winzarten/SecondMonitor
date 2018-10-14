namespace SecondMonitor.WindowsControls.WPF.CarSettingsControl
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using DataModel.BasicProperties;

    public class TyreCompoundSettingsControl : Control
    {
        private static readonly DependencyProperty CompoundNameProperty = DependencyProperty.Register("CompoundName", typeof(string), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty TemperatureUnitProperties = DependencyProperty.Register("TemperatureUnit", typeof(TemperatureUnits), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty MinimalIdealTyreTemperatureProperty = DependencyProperty.Register("MinimalIdealTyreTemperature", typeof(Temperature), typeof(TyreCompoundSettingsControl));

        private static readonly DependencyProperty MaximumIdealTyreTemperatureProperty = DependencyProperty.Register("MaximumIdealTyreTemperature", typeof(Temperature), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty MinimalIdealTyrePressureProperty = DependencyProperty.Register("MinimalIdealTyrePressure", typeof(Pressure), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty MaximumIdealTyrePressureProperty = DependencyProperty.Register("MaximumIdealTyrePressure", typeof(Pressure), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty IsGlobalCompoundProperty = DependencyProperty.Register("IsGlobalCompound", typeof(bool), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty CopyCompoundCommandProperty = DependencyProperty.Register("CopyCompoundCommand", typeof(ICommand), typeof(TyreCompoundSettingsControl));

        public TemperatureUnits TemperatureUnit
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitProperties);
            set => SetValue(TemperatureUnitProperties, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public string CompoundName
        {
            get => (string)GetValue(CompoundNameProperty);
            set => SetValue(CompoundNameProperty, value);
        }

        public bool IsGlobalCompound
        {
            get => (bool)GetValue(IsGlobalCompoundProperty);
            set => SetValue(IsGlobalCompoundProperty, value);
        }

        public Temperature MinimalIdealTyreTemperature
        {
            get => (Temperature)GetValue(MinimalIdealTyreTemperatureProperty);
            set => SetValue(MinimalIdealTyreTemperatureProperty, value);
        }

        public Temperature MaximumIdealTyreTemperature
        {
            get => (Temperature)GetValue(MaximumIdealTyreTemperatureProperty);
            set => SetValue(MaximumIdealTyreTemperatureProperty, value);
        }

        public Pressure MinimalIdealTyrePressure
        {
            get => (Pressure)GetValue(MinimalIdealTyrePressureProperty);
            set => SetValue(MinimalIdealTyrePressureProperty, value);
        }

        public Pressure MaximumIdealTyrePressure
        {
            get => (Pressure)GetValue(MaximumIdealTyrePressureProperty);
            set => SetValue(MaximumIdealTyrePressureProperty, value);
        }

        public ICommand CopyCompoundCommand
        {
            get => (ICommand)GetValue(CopyCompoundCommandProperty);
            set => SetValue(CopyCompoundCommandProperty, value);
        }
    }
}