namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.Windows;
    using DataModel.BasicProperties;

    public class VelocityText : AbstractQuantityText<Velocity>
    {
        public static readonly DependencyProperty VelocityUnitsProperty = DependencyProperty.Register("VelocityUnits", typeof(VelocityUnits), typeof(VelocityText), new PropertyMetadata(default(VelocityUnits)));

        public VelocityUnits VelocityUnits
        {
            get => (VelocityUnits) GetValue(VelocityUnitsProperty);
            set => SetValue(VelocityUnitsProperty, value);
        }

        protected override void UpdateIQuantity(double valueInUnits)
        {

        }

        protected override string GetUnitSymbol()
        {
            return Velocity.GetUnitSymbol(VelocityUnits);
        }

        protected override double GetValueInUnits()
        {
            return Quantity.GetValueInUnits(VelocityUnits);
        }
    }

}