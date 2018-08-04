namespace SecondMonitor.WindowsControls.WPF
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for TimeSpanBarControl.xaml
    /// </summary>
    public partial class TimeSpanBarControl : UserControl
    {

        private static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TimeSpanBarControl), new PropertyMetadata() { DefaultValue = "Title"});
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeSpanBarControl), new PropertyMetadata(TimeSpan.Zero, ValuePropertyChangedCallback));
        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(TimeSpan), typeof(TimeSpanBarControl), new PropertyMetadata(TimeSpan.FromSeconds(2), ValuePropertyChangedCallback));


        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public TimeSpan Value
        {
            get => (TimeSpan)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public TimeSpan MaxValue
        {
            get => (TimeSpan)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        private void UpdateByValue()
        {
            double sanitizedValue = Value.TotalSeconds;
            if (sanitizedValue > MaxValue.TotalSeconds)
            {
                sanitizedValue = MaxValue.TotalSeconds;
            }
            else if (sanitizedValue < -MaxValue.TotalSeconds)
            {
                sanitizedValue = -MaxValue.TotalSeconds;
            }

            double maxvaluePortion = sanitizedValue / MaxValue.TotalSeconds;
            if (maxvaluePortion > 0)
            {
                DeltaRectangle.RenderTransform = new TranslateTransform(0, 0);
                DeltaRectangle.Width = (ActualWidth / 2) * maxvaluePortion;
            }
            else
            {
                DeltaRectangle.RenderTransform = new TranslateTransform((ActualWidth / 2) * maxvaluePortion, 0);
                DeltaRectangle.Width = (ActualWidth / 2) * -maxvaluePortion;
            }
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as TimeSpanBarControl)?.UpdateByValue();

        public TimeSpanBarControl()
        {
            InitializeComponent();
        }
    }
}
