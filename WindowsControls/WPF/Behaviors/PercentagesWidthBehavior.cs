namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class PercentagesWidthBehavior : Behavior<FrameworkElement>
    {
        private static readonly DependencyProperty PercentageProperty = DependencyProperty.Register("Percentage", typeof(double), typeof(PercentagesWidthBehavior), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnPercentagesPropertyChanged});

        private FrameworkElement _parentElement;

        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        public PercentagesWidthBehavior()
        {

        }

        protected override void OnAttached()
        {
            _parentElement = (FrameworkElement)LogicalTreeHelper.GetParent(AssociatedObject);
        }

        private void UpdateWidth()
        {
            if (AssociatedObject == null || _parentElement == null)
            {
                return;
            }

            double computedWith = _parentElement.ActualWidth * (Percentage / 100);
            if (computedWith > 0)
            {
                AssociatedObject.Width = computedWith;
            }
        }

        private static void OnPercentagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PercentagesWidthBehavior percentagesWidthBehavior)
            {
                percentagesWidthBehavior.UpdateWidth();
            }
        }
    }
}