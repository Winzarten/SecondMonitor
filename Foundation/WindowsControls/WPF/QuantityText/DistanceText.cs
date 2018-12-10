namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;
    using DataModel.BasicProperties;

    public class DistanceText : AbstractQuantityText<Distance>
    {
        private static readonly DependencyProperty DistanceUnitsProperty = DependencyProperty.Register("DistanceUnits", typeof(DistanceUnits), typeof(DistanceText), new FrameworkPropertyMetadata(){PropertyChangedCallback = QuantityChanged});

        public DistanceUnits DistanceUnits
        {
            get => (DistanceUnits) GetValue(DistanceUnitsProperty);
            set => SetValue(DistanceUnitsProperty, value);
        }

        protected override void UpdateIQuantity(double valueInUnits)
        {
            
        }

        protected override string GetUnitSymbol()
        {
            return Distance.GetUnitsSymbol(DistanceUnits);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetByUnit(DistanceUnits);
        }
    }
}