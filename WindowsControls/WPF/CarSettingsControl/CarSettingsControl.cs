﻿namespace SecondMonitor.WindowsControls.WPF.CarSettingsControl
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using SecondMonitor.DataModel.BasicProperties;

    public class CarSettingsControl : Control
    {
        private static readonly DependencyProperty IdealBrakeTemperatureProperty = DependencyProperty.Register("IdealBrakeTemperature", typeof(Temperature), typeof(CarSettingsControl));
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(CarSettingsControl));
        private static readonly DependencyProperty IdealBrakeTemperatureWindowProperty = DependencyProperty.Register("IdealBrakeTemperatureWindow", typeof(Temperature), typeof(CarSettingsControl));
        private static readonly DependencyProperty TemperatureUnitProperties = DependencyProperty.Register("TemperatureUnit", typeof(TemperatureUnits), typeof(CarSettingsControl));
        private static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName",typeof(string), typeof(CarSettingsControl));
        private static readonly DependencyProperty OkCommandProperty = DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(CarSettingsControl));
        private static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(CarSettingsControl));
        private static readonly DependencyProperty TyreCompoundsProperty = DependencyProperty.Register("TyreCompounds", typeof(IReadOnlyCollection<ITyreSettingsViewModel>), typeof(CarSettingsControl));
        private static readonly DependencyProperty SelectedTyreCompoundProperty = DependencyProperty.Register("SelectedTyreCompound", typeof(ITyreSettingsViewModel), typeof(CarSettingsControl));
        private static readonly DependencyProperty CopyCompoundCommandProperty = DependencyProperty.Register("CopyCompoundCommand", typeof(ICommand), typeof(CarSettingsControl));

        public Temperature IdealBrakeTemperature
        {
            get => (Temperature)GetValue(IdealBrakeTemperatureProperty);
            set => SetValue(IdealBrakeTemperatureProperty, value);
        }

        public Temperature IdealBrakeTemperatureWindow
        {
            get => (Temperature)GetValue(IdealBrakeTemperatureWindowProperty);
            set => SetValue(IdealBrakeTemperatureWindowProperty, value);
        }

        public TemperatureUnits TemperatureUnit
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitProperties);
            set => SetValue(TemperatureUnitProperties, value);
        }

        public string CarName
        {
            get => (string)GetValue(CarNameProperty);
            set => SetValue(CarNameProperty, value);
        }

        public IReadOnlyCollection<ITyreSettingsViewModel> TyreCompounds
        {
            get => (IReadOnlyCollection<ITyreSettingsViewModel>)GetValue(TyreCompoundsProperty);
            set => SetValue(TyreCompoundsProperty, value);
        }

        public ITyreSettingsViewModel SelectedTyreCompound
        {
            get => (ITyreSettingsViewModel)GetValue(SelectedTyreCompoundProperty);
            set => SetValue(SelectedTyreCompoundProperty, value);
        }

        public ICommand OkCommand
        {
            get => (ICommand)GetValue(OkCommandProperty);
            set => SetValue(OkCommandProperty, value);
        }

        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public ICommand CopyCompoundCommand
        {
            get => (ICommand)GetValue(CopyCompoundCommandProperty);
            set => SetValue(CopyCompoundCommandProperty, value);
        }
    }
}