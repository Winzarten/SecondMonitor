namespace SecondMonitor.WindowsControls.WPF.QuantityText
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    using SecondMonitor.DataModel.BasicProperties;

    public abstract class AbstractQuantityText<T> : Control where T : class, IQuantity
    {
        private static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(T), typeof(AbstractQuantityText<T>), new PropertyMetadata() { PropertyChangedCallback = QuantityChanged });
        private static readonly DependencyProperty ValueInUnitsProperty = DependencyProperty.Register("ValueInUnits", typeof(double), typeof(AbstractQuantityText<T>), new PropertyMetadata());

        public event PropertyChangedEventHandler PropertyChanged;

        public double ValueInUnits
        {
            get => (double)GetValue(ValueInUnitsProperty);
            set => SetValue(ValueInUnitsProperty, value);
        }

        public T Quantity
        {
            get => (T)GetValue(QuantityProperty);
            set => SetValue(QuantityProperty, value);
        }

        private void UpdateControl()
        {
            if (Quantity == null)
            {
                return;
            }

            ValueInUnits = GetValueInUnits();
        }

        protected abstract double GetValueInUnits();

        protected static void QuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AbstractQuantityText<T> abstractQuantityText)
            {
                abstractQuantityText.UpdateControl();
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}