namespace SecondMonitor.TelemetryPresentation.Behavior
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using OxyPlot.Wpf;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using OxyPlot;
    using OxyPlot.Axes;
    using Telemetry.TelemetryApplication.ViewModels.GraphPanel;
    using Axis = OxyPlot.Axes.Axis;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using VerticalAlignment = System.Windows.VerticalAlignment;

    public class ShowSelectedDistanceBehavior : Behavior<PlotView>
    {

        private Rectangle _rectangle;
        private TranslateTransform _transform;

        public static readonly DependencyProperty GraphViewModelProperty = DependencyProperty.Register(
            "GraphViewModel", typeof(AbstractGraphViewModel), typeof(ShowSelectedDistanceBehavior), new PropertyMetadata(){ PropertyChangedCallback = GraphViewModelPropertyChangedCallback});

        public AbstractGraphViewModel GraphViewModel
        {
            get => (AbstractGraphViewModel) GetValue(GraphViewModelProperty);
            set => SetValue(GraphViewModelProperty, value);
        }

        protected override void OnAttached()
        {
            SubscribeToDataContextChange();
            AddRectangle();
        }

        private void AddRectangle()
        {
            _transform = new TranslateTransform();
            _rectangle = new Rectangle()
            {
                Margin = new Thickness(0, 0, 0, 0),
                Height = 300,
                Width = 1.5,
                Fill = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                RenderTransform = _transform
            };

            Grid grid = VisualTreeHelper.GetParent(AssociatedObject) as Grid;
            grid?.Children.Add(_rectangle);
        }

        protected override void OnDetaching()
        {
            UnSubscribeToDataContextChange();
        }

        private void SubscribeToDataContextChange()
        {
            AssociatedObject.DataContextChanged += AssociatedObjectOnDataContextChanged;
            SubscribeToPlot();
        }

        private void SubscribeToPlot()
        {
            if (AssociatedObject?.Model == null)
            {
                return;
            }
            AssociatedObject.Model.Axes.CollectionChanged += AxesOnCollectionChanged;
        }

        private void AxesOnCollectionChanged(object sender, ElementCollectionChangedEventArgs<Axis> e)
        {
            foreach (Axis eAddedItem in e.AddedItems)
            {
                eAddedItem.AxisChanged += AxisChange;
            }
        }

        private void AxisChange(object sender, AxisChangedEventArgs e)
        {
            UpdateRectangle();
        }

        private void UnSubscribeToDataContextChange()
        {
            AssociatedObject.DataContextChanged -= AssociatedObjectOnDataContextChanged;

        }

        private void AssociatedObjectOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void GraphViewModelPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ShowSelectedDistanceBehavior showSelectedDistance))
            {
                return;
            }
            if (e.OldValue is AbstractGraphViewModel oldGraphViewModel)
            {
                oldGraphViewModel.PropertyChanged -= showSelectedDistance.GraphViewModelPropertyChanged;
            }

            if (e.NewValue is AbstractGraphViewModel newValue)
            {
                newValue.PropertyChanged += showSelectedDistance.GraphViewModelPropertyChanged;
            }
        }

        private void GraphViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(AbstractGraphViewModel.SelectedDistance))
            {
                return;
            }
            UpdateRectangle();
        }

        private void UpdateRectangle()
        {
            PlotModel model = GetPlotModel();
            if (model == null || model.Axes.Count != 2)
            {
                return;
            }

            Axis xAxis = model.Axes.FirstOrDefault(x => x.Position == AxisPosition.Bottom);

            if (xAxis == null)
            {
                return;
            }
            _rectangle.Height = model.PlotArea.Height;
            double distanceInUnits = GraphViewModel.SelectedDistance.GetByUnit(GraphViewModel.DistanceUnits);
            if (xAxis.ActualMinimum > distanceInUnits || distanceInUnits > xAxis.ActualMaximum)
            {
                _rectangle.Visibility = Visibility.Hidden;
                return;
            }

            _rectangle.Visibility = Visibility.Visible;
            double plotRange = xAxis.ActualMaximum - xAxis.ActualMinimum;
            double selectedDistancePortion = (distanceInUnits - xAxis.ActualMinimum) / plotRange;
            _transform.Y = model.PlotArea.Top;
            _transform.X = model.PlotArea.Left + model.PlotArea.Width * selectedDistancePortion;

        }

        private PlotModel GetPlotModel()
        {
            return AssociatedObject.Model;
        }
    }
}