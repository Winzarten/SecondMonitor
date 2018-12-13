using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using SecondMonitor.WindowsControls.Colors;

namespace SecondMonitor.WindowsControls.WPF.Behaviors.TyreWear
{
    public abstract class ColorByLinearLimitsBehavior<T> : Behavior<T> where T : UIElement
    {

        private static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register("DefaultColor", typeof(Color), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty IdealLimitColorProperty = DependencyProperty.Register("IdealLimitColor", typeof(Color), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty MildLimitColorProperty = DependencyProperty.Register("MildLimitColor", typeof(Color), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty HeavyLimitColorProperty = DependencyProperty.Register("HeavyLimitColor", typeof(Color), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",typeof(double), typeof(ColorByLinearLimitsBehavior<T>), new PropertyMetadata() {PropertyChangedCallback = OnValueChanged});

        private static readonly DependencyProperty IdealLimitProperty = DependencyProperty.Register("IdealLimit", typeof(double), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty MildLimitProperty = DependencyProperty.Register("MildLimit", typeof(double), typeof(ColorByLinearLimitsBehavior<T>));
        private static readonly DependencyProperty HeavyLimitProperty = DependencyProperty.Register("HeavyLimit", typeof(double), typeof(ColorByLinearLimitsBehavior<T>));

        private double _oldValue = double.NaN;

        public Color DefaultColor
        {
            get => (Color) GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        public Color IdealLimitColor
        {
            get => (Color)GetValue(IdealLimitColorProperty);
            set => SetValue(IdealLimitColorProperty, value);
        }

        public Color MildLimitColor
        {
            get => (Color)GetValue(MildLimitColorProperty);
            set => SetValue(MildLimitColorProperty, value);
        }

        public Color HeavyLimitColor
        {
            get => (Color)GetValue(HeavyLimitColorProperty);
            set => SetValue(HeavyLimitColorProperty, value);
        }

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double IdealLimit
        {
            get => (double)GetValue(IdealLimitProperty);
            set => SetValue(IdealLimitProperty, value);
        }

        public double MildLimit
        {
            get => (double)GetValue(MildLimitProperty);
            set => SetValue(MildLimitProperty, value);
        }

        public double HeavyLimit
        {
            get => (double)GetValue(HeavyLimitProperty);
            set => SetValue(HeavyLimitProperty, value);
        }

        protected abstract void ApplyColor(Color color);

        protected void UpdateColor()
        {
            if (!double.IsNaN(_oldValue) && Math.Abs(_oldValue - Value) < 0.1)
            {
                return;
            }


            Color color = ComputeColor();
            ApplyColor(color);
            _oldValue = Value;
        }

        private Color ComputeColor()
        {
            if (Value >= IdealLimit)
            {
                return IdealLimitColor;
            }

            if (Value <= HeavyLimit)
            {
                return HeavyLimitColor;
            }

            if (MildLimit <= Value &&  Value <= IdealLimit )
            {
                double percentage = (Value - MildLimit) / (IdealLimit - MildLimit);
                Color color  = MediaColorExtension.InterpolateHslColor(MildLimitColor, IdealLimitColor, percentage);
                return color;
            }
            else
            {
                double percentage = (Value - HeavyLimit) / (MildLimit - HeavyLimit);
                Color color = MediaColorExtension.InterpolateHslColor(HeavyLimitColor, MildLimitColor, percentage);
                return color;
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorByLinearLimitsBehavior<T> behavior)
            {
                behavior.OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            UpdateColor();
        }
    }
}