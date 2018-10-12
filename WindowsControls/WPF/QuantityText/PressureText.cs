namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;

    using SecondMonitor.DataModel.BasicProperties;

    public class PressureText : AbstractQuantityText<Pressure>
    {
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(PressureText), new PropertyMetadata { DefaultValue = PressureUnits.Kpa, PropertyChangedCallback = QuantityChanged });

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        protected override void UpdateIQuantity(double valueInUnits)
        {
            Quantity.UpdateValue(valueInUnits, PressureUnits);
        }

        protected override string GetUnitSymbol()
        {
            return Pressure.GetUnitSymbol(PressureUnits);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetValueInUnits(PressureUnits);
        }
    }
}