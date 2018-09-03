namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class PercentagesHeightBehavior : Behavior<FrameworkElement>
    {
        private static readonly DependencyProperty PercentageProperty = DependencyProperty.Register("Percentage", typeof(double), typeof(PercentagesHeightBehavior), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnPercentagesPropertyChanged });

        private FrameworkElement _parentElement;

        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        protected override void OnAttached()
        {
            _parentElement = (FrameworkElement)LogicalTreeHelper.GetParent(AssociatedObject);
        }

        private void UpdateHeight()
        {
            if (AssociatedObject == null || _parentElement == null)
            {
                return;
            }

            AssociatedObject.Height = _parentElement.ActualHeight * (Percentage / 100);
        }

        private static void OnPercentagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PercentagesHeightBehavior percentagesHeightBehavior)
            {
                percentagesHeightBehavior.UpdateHeight();
            }
        }
    }
}