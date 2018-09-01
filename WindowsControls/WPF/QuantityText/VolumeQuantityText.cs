﻿namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;

    using SecondMonitor.DataModel.BasicProperties;

    public class VolumeQuantityText : AbstractQuantityText<Volume>
    {
        private static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(VolumeQuantityText), new PropertyMetadata { DefaultValue = VolumeUnits.Liters, PropertyChangedCallback = QuantityChanged });

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits)GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetValueInUnits(VolumeUnits);
        }
    }
}