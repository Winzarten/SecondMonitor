namespace SecondMonitor.WindowsControls.WPF.CarSettingsControl
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.WindowsControls.Annotations;

    public class TyreCompoundSettingsControl : Control
    {
        private static readonly DependencyProperty CompoundNameProperty = DependencyProperty.Register("CompoundName", typeof(string), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty TemperatureUnitProperties = DependencyProperty.Register("TemperatureUnit", typeof(TemperatureUnits), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty IdealTyreTemperatureProperty = DependencyProperty.Register("IdealTyreTemperature", typeof(Temperature), typeof(TyreCompoundSettingsControl));

        private static readonly DependencyProperty IdealTyreTemperatureWindowProperty = DependencyProperty.Register("IdealTyreTemperatureWindow", typeof(Temperature), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty IdealTyrePressureProperty = DependencyProperty.Register("IdealTyrePressure", typeof(Pressure), typeof(TyreCompoundSettingsControl));
        private static readonly DependencyProperty IdealTyrePressureWindowProperty = DependencyProperty.Register("IdealTyrePressureWindow", typeof(Pressure), typeof(TyreCompoundSettingsControl));
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

        public Temperature IdealTyreTemperature
        {
            get => (Temperature)GetValue(IdealTyreTemperatureProperty);
            set => SetValue(IdealTyreTemperatureProperty, value);
        }

        public Temperature IdealTyreTemperatureWindow
        {
            get => (Temperature)GetValue(IdealTyreTemperatureWindowProperty);
            set => SetValue(IdealTyreTemperatureWindowProperty, value);
        }

        public Pressure IdealTyrePressure
        {
            get => (Pressure)GetValue(IdealTyrePressureProperty);
            set => SetValue(IdealTyrePressureProperty, value);
        }

        public Pressure IdealTyrePressureWindow
        {
            get => (Pressure)GetValue(IdealTyrePressureWindowProperty);
            set => SetValue(IdealTyrePressureWindowProperty, value);
        }

        public ICommand CopyCompoundCommand
        {
            get => (ICommand)GetValue(CopyCompoundCommandProperty);
            set => SetValue(CopyCompoundCommandProperty, value);
        }
    }
}