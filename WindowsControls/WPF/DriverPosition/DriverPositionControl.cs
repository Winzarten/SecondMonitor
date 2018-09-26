namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class DriverPositionControl : Control
    {
        private static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",typeof(int), typeof(DriverPositionControl));
        private static readonly DependencyProperty CircleBrushProperty = DependencyProperty.Register("CircleBrush", typeof(SolidColorBrush), typeof(DriverPositionControl));
        private static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register("TextBrush", typeof(SolidColorBrush), typeof(DriverPositionControl));
        private static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(DriverPositionControl), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnXPropertyChanged });
        private static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(DriverPositionControl), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnYPropertyChanged });

        private static readonly TimeSpan AnimationTime = TimeSpan.FromMilliseconds(300);

        private readonly TranslateTransform _translateTransform;

        public DriverPositionControl()
        {
            _translateTransform = new TranslateTransform(0,0);
            RenderTransform = _translateTransform;
        }

        public SolidColorBrush CircleBrush
        {
            get => (SolidColorBrush)GetValue(CircleBrushProperty);
            set => SetValue(CircleBrushProperty, value);
        }

        public SolidColorBrush TextBrush
        {
            get => (SolidColorBrush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public int Position
        {
            get => (int)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        private static void OnYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DriverPositionControl playerPositionControl)
            {
                playerPositionControl.OnYPropertyChanged();
            }
        }

        private static void OnXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DriverPositionControl playerPositionControl)
            {
                playerPositionControl.OnXPropertyChanged();
            }
        }

        private void OnYPropertyChanged()
        {
            _translateTransform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(Y, AnimationTime), HandoffBehavior.SnapshotAndReplace);
            //_translateTransform.Y = Y;
        }

        private void OnXPropertyChanged()
        {
            _translateTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(X, AnimationTime), HandoffBehavior.SnapshotAndReplace);
            //_translateTransform.X = X;
        }
    }
}