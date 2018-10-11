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
        private static readonly DependencyProperty UnitSymbolProperty = DependencyProperty.Register("UnitSymbol", typeof(string), typeof(AbstractQuantityText<T>), new PropertyMetadata());
        private static readonly DependencyProperty ShowUnitSymbolProperty = DependencyProperty.Register("ShowUnitSymbol", typeof(bool), typeof(AbstractQuantityText<T>), new PropertyMetadata(false) { PropertyChangedCallback = QuantityChanged });
        private static readonly DependencyProperty IsReadonlyProperty = DependencyProperty.Register("IsReadonly", typeof(bool), typeof(AbstractQuantityText<T>), new PropertyMetadata(true));
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

        public bool ShowUnitSymbol
        {
            get => (bool)GetValue(ShowUnitSymbolProperty);
            set => SetValue(ShowUnitSymbolProperty, value);
        }

        public bool IsReadonly
        {
            get => (bool)GetValue(IsReadonlyProperty);
            set => SetValue(IsReadonlyProperty, value);
        }

        public string UnitSymbol
        {
            get => (string)GetValue(UnitSymbolProperty);
            set => SetValue(UnitSymbolProperty, value);
        }

        protected static void QuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AbstractQuantityText<T> abstractQuantityText)
            {
                abstractQuantityText.UpdateControl();
            }
        }

        protected abstract string GetUnitSymbol();

        protected abstract double GetValueInUnits();

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateControl()
        {
            if (Quantity == null)
            {
                return;
            }

            UnitSymbol = GetUnitSymbol();
            ValueInUnits = GetValueInUnits();
        }
    }
}