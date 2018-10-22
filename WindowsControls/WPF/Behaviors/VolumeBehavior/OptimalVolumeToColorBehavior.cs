using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.WindowsControls.Colors;

namespace SecondMonitor.WindowsControls.WPF.Behaviors.VolumeBehavior
{
    public abstract class OptimalVolumeToColorBehavior<T,TV> : Behavior<TV> where T : class, IQuantity, new() where TV : UIElement
    {
        private static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof(OptimalQuantity<T>), typeof(OptimalVolumeToColorBehavior<T,TV>), new PropertyMetadata(){ PropertyChangedCallback = OptimalVolumeChanged<T,TV> });

        private static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register("DefaultColor", typeof(Color), typeof(OptimalVolumeToColorBehavior<T,TV>));
        private static readonly DependencyProperty LowQuantityColorProperty = DependencyProperty.Register("LowQuantityColor", typeof(Color), typeof(OptimalVolumeToColorBehavior<T,TV>));
        private static readonly DependencyProperty IdealQuantityColorProperty = DependencyProperty.Register("IdealQuantityColor", typeof(Color), typeof(OptimalVolumeToColorBehavior<T,TV>));
        private static readonly DependencyProperty HighQuantityColorProperty = DependencyProperty.Register("HighQuantityColor", typeof(Color), typeof(OptimalVolumeToColorBehavior<T,TV>));

        private double _oldOptimalValue = double.NaN;

        private Color _oldComputedColor;

        public OptimalQuantity<T> Volume
        {
            get => (OptimalQuantity<T>)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public Color DefaultColor
        {
            get => (Color)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        public Color LowQuantityColor
        {
            get => (Color)GetValue(LowQuantityColorProperty);
            set => SetValue(LowQuantityColorProperty, value);
        }

        public Color IdealQuantityColor
        {
            get => (Color)GetValue(IdealQuantityColorProperty);
            set => SetValue(IdealQuantityColorProperty, value);
        }

        public Color HighQuantityColor
        {
            get => (Color)GetValue(HighQuantityColorProperty);
            set => SetValue(HighQuantityColorProperty, value);
        }

        protected void UpdateColor()
        {
            Color color;
            if (Volume == null || AssociatedObject == null)
            {
                color = _oldComputedColor;
            }
            else if (Volume.IdealQuantity.IsZero || Volume.IdealQuantityWindow.IsZero)
            {
                color = DefaultColor;
            }
            else
            {
                color = ComputeColor(
                    Volume.ActualQuantity.RawValue,
                    Volume.IdealQuantity.RawValue,
                    Volume.IdealQuantityWindow.RawValue);
            }

            _oldComputedColor = color;
            ApplyColor(color);
        }

        private Color ComputeColor(double value, double optimalValue, double window)
        {
            if (optimalValue == _oldOptimalValue)
            {
                return _oldComputedColor;
            }

            double threshold = window * 2;
            double downThreshold = window * 2;

            if (value < optimalValue - window - downThreshold)
            {
                _oldComputedColor = LowQuantityColor;
                return _oldComputedColor;
            }

            if (value > optimalValue + window + threshold)
            {
                _oldComputedColor = HighQuantityColor;
                return _oldComputedColor;
            }

            if (value > optimalValue - window && value < optimalValue + window)
            {
                _oldComputedColor = IdealQuantityColor;
                return _oldComputedColor;
            }

            if (value < optimalValue)
            {
                double percentage = (value - (optimalValue - window - downThreshold))
                                    / (optimalValue - window - (optimalValue - window - downThreshold));
                _oldComputedColor = MediaColorExtension.InterpolateHslColor(LowQuantityColor, IdealQuantityColor, percentage);
                return _oldComputedColor;
            }
            else
            {
                double percentage = ((optimalValue + window) - value)
                                    / (optimalValue + window - (optimalValue + window + threshold));

                _oldComputedColor = MediaColorExtension.InterpolateHslColor(IdealQuantityColor, HighQuantityColor, percentage);
                return _oldComputedColor;
            }
        }

        protected abstract void ApplyColor(Color color);

        private static void OptimalVolumeChanged<A,B>(DependencyObject d, DependencyPropertyChangedEventArgs e) where A : class, IQuantity, new() where B : UIElement
        {
            if (d is OptimalVolumeToColorBehavior<A,B> optimalVolumeToColorBehavior)
            {
                optimalVolumeToColorBehavior.UpdateColor();
            }
        }
    }
}