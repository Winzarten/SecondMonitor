namespace SecondMonitor.WindowsControls.WPF
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Extension;

    public class ColorAbleIcon : Viewbox
    {
        private static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(ColorAbleIcon), new FrameworkPropertyMetadata(){DefaultValue = Brushes.White});

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        public static readonly DependencyProperty RebindOnVisibilityChangeProperty = DependencyProperty.Register(
            "RebindOnVisibilityChange", typeof(bool), typeof(ColorAbleIcon), new PropertyMetadata(default(bool)));

        public bool RebindOnVisibilityChange
        {
            get => (bool) GetValue(RebindOnVisibilityChangeProperty);
            set => SetValue(RebindOnVisibilityChangeProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!RebindOnVisibilityChange|| !IsVisible)
            {
                return;
            }

            foreach (var objectWithProperty in this.GetAllDescendantWithProperty(Shape.FillProperty.Name))
            {
                if (!(objectWithProperty is Shape shape))
                {
                    continue;
                }
                BindingOperations.ClearBinding(shape, Shape.FillProperty);
                Binding newBinding = new Binding();
                newBinding.Source = this;
                newBinding.Path = new PropertyPath(StrokeBrushProperty);
                newBinding.Mode = BindingMode.OneWay;
                newBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(shape, Shape.FillProperty, newBinding);
            }

        }
    }
}