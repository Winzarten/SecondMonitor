﻿namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;

    using DataModel.BasicProperties;

    public class TemperatureText : AbstractQuantityText<Temperature>
    {
        private static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(TemperatureText), new PropertyMetadata { DefaultValue = TemperatureUnits.Celsius, PropertyChangedCallback = QuantityChanged });

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        protected override void UpdateIQuantity(double valueInUnits)
        {
            Quantity.UpdateValue(valueInUnits, TemperatureUnits);
        }

        protected override string GetUnitSymbol()
        {
            return Temperature.GetUnitSymbol(TemperatureUnits);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetValueInUnits(TemperatureUnits);
        }
    }
}