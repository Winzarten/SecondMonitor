namespace SecondMonitor.WindowsControls.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ColorAbleIcon : Viewbox
    {
        private static readonly DependencyProperty StrokeBrushProperty =  DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(ColorAbleIcon));

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }
    }
}