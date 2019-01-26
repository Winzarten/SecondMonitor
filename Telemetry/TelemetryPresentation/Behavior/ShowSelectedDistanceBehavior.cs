namespace SecondMonitor.TelemetryPresentation.Behavior
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using OxyPlot.Wpf;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using DataModel.BasicProperties;
    using OxyPlot;
    using OxyPlot.Axes;
    using Telemetry.TelemetryApplication.ViewModels.GraphPanel;
    using Axis = OxyPlot.Axes.Axis;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using VerticalAlignment = System.Windows.VerticalAlignment;

    public class ShowSelectedDistanceBehavior : Behavior<PlotView>
    {
        private Dictionary<string, (Rectangle rectangle, TranslateTransform transform)> _lapRectangles;

        public static readonly DependencyProperty GraphViewModelProperty = DependencyProperty.Register(
            "GraphViewModel", typeof(AbstractGraphViewModel), typeof(ShowSelectedDistanceBehavior), new PropertyMetadata() {PropertyChangedCallback = GraphViewModelPropertyChangedCallback});

        public ShowSelectedDistanceBehavior()
        {
            _lapRectangles = new Dictionary<string, (Rectangle rectangle, TranslateTransform transform)>();
        }

        public AbstractGraphViewModel GraphViewModel
        {
            get => (AbstractGraphViewModel) GetValue(GraphViewModelProperty);
            set => SetValue(GraphViewModelProperty, value);
        }

        protected override void OnAttached()
        {
            SubscribeToDataContextChange();
        }

        private void AddRectangle(string lapId, Color color, double xValue)
        {
            TranslateTransform transform = new TranslateTransform();
            Rectangle rectangle = new Rectangle()
            {
                Margin = new Thickness(0, 0, 0, 0),
                Height = 300,
                Width = 1.5,
                Fill = new SolidColorBrush(color),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                RenderTransform = transform
            };

            Grid grid = VisualTreeHelper.GetParent(AssociatedObject) as Grid;
            grid?.Children.Add(rectangle);
            _lapRectangles[lapId] = (rectangle, transform);
            UpdateRectangle(rectangle, transform, xValue, color);
        }

        private void RemoveRectangle(string lapId)
        {
            if (!_lapRectangles.TryGetValue(lapId, out (Rectangle rectangle, TranslateTransform transform) value))
            {
                return;
            }

            Grid grid = VisualTreeHelper.GetParent(AssociatedObject) as Grid;
            grid?.Children.Remove(value.rectangle);
            _lapRectangles.Remove(lapId);
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
            UpdateRectangles();
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
            if (e.PropertyName != nameof(AbstractGraphViewModel.SelectedDistances))
            {
                return;
            }

            UpdateRectangles();
        }


        private void UpdateRectangles()
        {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, (Rectangle rectangle, TranslateTransform transform)> lapRectanglesValue in _lapRectangles)
            {
                if (!GraphViewModel.SelectedDistances.TryGetValue(lapRectanglesValue.Key, out (double x, Color color) lapDistance))
                {
                    keysToRemove.Add(lapRectanglesValue.Key);
                    continue;
                }
                UpdateRectangle(lapRectanglesValue.Value.rectangle,lapRectanglesValue.Value.transform, lapDistance.x, lapDistance.color);
            }

            foreach (string lapId in GraphViewModel.SelectedDistances.Keys.Where(x => !_lapRectangles.Keys.Contains(x)))
            {
                AddRectangle(lapId, GraphViewModel.SelectedDistances[lapId].color, GraphViewModel.SelectedDistances[lapId].x);
            }

            keysToRemove.ForEach(RemoveRectangle);
        }

        private void UpdateRectangle(Rectangle rectangle, TranslateTransform translateTransform, double xValue, Color color)
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

            if (((SolidColorBrush)rectangle.Fill).Color != color)
            {
                rectangle.Fill = new SolidColorBrush(color);
            }

            rectangle.Height = model.PlotArea.Height;
            if (xAxis.ActualMinimum > xValue || xValue > xAxis.ActualMaximum)
            {
                rectangle.Visibility = Visibility.Hidden;
                return;
            }

            rectangle.Visibility = Visibility.Visible;
            double plotRange = xAxis.ActualMaximum - xAxis.ActualMinimum;
            double selectedDistancePortion = (xValue - xAxis.ActualMinimum) / plotRange;
            translateTransform.Y = model.PlotArea.Top;
            translateTransform.X = model.PlotArea.Left + model.PlotArea.Width * selectedDistancePortion;
        }

        private PlotModel GetPlotModel()
        {
            return AssociatedObject.Model;
        }
    }
}