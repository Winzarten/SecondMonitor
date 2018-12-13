namespace SecondMonitor.WindowsControls.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ColorAbleIcon : Viewbox
    {
        private static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(ColorAbleIcon), new FrameworkPropertyMetadata());

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

       /* private void OnBrushChanged()
        {
            List<Path> paths = FindPaths(this);
            paths.ForEach(SetFillBrush);
        }

        internal List<Path> FindPaths(Viewbox symbol)
        {
            List<Path> paths = new List<Path>();
            FindPaths(symbol.Child, paths);

            return paths;
        }

        private void SetFillBrush(Path path)
        {
            if (StrokeBrush == null)
            {
                return;
            }

            Binding bd = new Binding(StrokeBrushProperty.Name);
            bd.Source = this;
            path.SetBinding(Shape.FillProperty, bd);
        }

        private void FindPaths(UIElement element, List<Path> paths)
        {
            if (element == null)
            {
                return;
            }

            Path path = element as Path;
            if (path != null)
            {
                paths.Add(path);
                return;
            }

            Panel panel = element as Panel;
            if (panel != null)
            {
                foreach (UIElement child in panel.Children)
                {
                    FindPaths(child, paths);
                }

                return;
            }

            ContentControl ct = element as ContentControl;
            if (ct != null)
            {
                FindPaths(ct, paths);
            }
        }

        private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorAbleIcon)d).OnBrushChanged();
        }*/
    }
}