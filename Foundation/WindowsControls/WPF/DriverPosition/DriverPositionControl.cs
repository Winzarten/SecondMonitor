﻿namespace SecondMonitor.WindowsControls.WPF.DriverPosition
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
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(DriverPositionControl), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnXPropertyChanged });
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(DriverPositionControl), new FrameworkPropertyMetadata() {PropertyChangedCallback = OnYPropertyChanged});
        public static readonly DependencyProperty OutLineColorProperty = DependencyProperty.Register("OutLineColor", typeof(SolidColorBrush), typeof(DriverPositionControl), new PropertyMetadata(Brushes.Transparent));

        private TranslateTransform _translateTransform;
        private TimeSpan _animationTime = TimeSpan.FromMilliseconds(100);

        public static readonly DependencyProperty ClassIndicationBrushProperty = DependencyProperty.Register(
            "ClassIndicationBrush", typeof(SolidColorBrush), typeof(DriverPositionControl), new PropertyMetadata(default(SolidColorBrush)));

        public static readonly DependencyProperty IsClassColorIndicationEnabledProperty = DependencyProperty.Register(
            "IsClassColorIndicationEnabled", typeof(bool), typeof(DriverPositionControl), new PropertyMetadata(default(bool)));

        public bool IsClassColorIndicationEnabled
        {
            get => (bool) GetValue(IsClassColorIndicationEnabledProperty);
            set => SetValue(IsClassColorIndicationEnabledProperty, value);
        }

        public SolidColorBrush ClassIndicationBrush
        {
            get => (SolidColorBrush) GetValue(ClassIndicationBrushProperty);
            set => SetValue(ClassIndicationBrushProperty, value);
        }

        public SolidColorBrush OutLineColor
        {
            get => (SolidColorBrush) GetValue(OutLineColorProperty);
            set => SetValue(OutLineColorProperty, value);
        }

        private bool _animate;

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

        public bool Animate
        {
            set
            {
                _animate = value;
                _translateTransform = new TranslateTransform(_translateTransform.X, _translateTransform.Y);
                RenderTransform = _translateTransform;
                _animationTime = value ? TimeSpan.FromMilliseconds(100) : TimeSpan.Zero;
            }

            get => _animate;
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
            if (Animate)
            {
                _translateTransform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(Y, _animationTime), HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                _translateTransform.Y = Y;
            }
        }

        private void OnXPropertyChanged()
        {
            if (Animate)
            {
                _translateTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(X, _animationTime), HandoffBehavior.SnapshotAndReplace);

            }
            else
            {
                _translateTransform.X = X;
            }
        }
    }
}