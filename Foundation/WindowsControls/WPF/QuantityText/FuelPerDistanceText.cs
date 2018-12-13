namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;
    using DataModel.BasicProperties.FuelConsumption;

    public class FuelPerDistanceText : AbstractQuantityText<FuelPerDistance>
    {
        private static readonly DependencyProperty FuelPerDistanceUnitsProperty = DependencyProperty.Register("FuelPerDistanceUnits", typeof(FuelPerDistanceUnits), typeof(FuelPerDistanceText), new FrameworkPropertyMetadata(){ PropertyChangedCallback = QuantityChanged});

        public FuelPerDistanceUnits FuelPerDistanceUnits
        {
            get => (FuelPerDistanceUnits) GetValue(FuelPerDistanceUnitsProperty);
            set => SetValue(FuelPerDistanceUnitsProperty, value);
        }

        protected override void UpdateIQuantity(double valueInUnits)
        {          
        }

        protected override string GetUnitSymbol()
        {
            return FuelPerDistance.GetUnitsSymbol(FuelPerDistanceUnits);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetConsumption(FuelPerDistanceUnits);
        }
    }
}