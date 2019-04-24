namespace SecondMonitor.TelemetryPresentation.Behavior
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using OxyPlot.Wpf;
    using Telemetry.TelemetryApplication.ViewModels.AggregatedCharts.Histogram;

    public class BandSelectionByClickBehavior : Behavior<PlotView>
    {
        public static readonly DependencyProperty HistogramChartViewModelProperty = DependencyProperty.Register(
            "HistogramChartViewModel", typeof(HistogramChartViewModel), typeof(BandSelectionByClickBehavior));

        public HistogramChartViewModel HistogramChartViewModel
        {
            get => (HistogramChartViewModel)GetValue(HistogramChartViewModelProperty);
            set => SetValue(HistogramChartViewModelProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnMouseUp;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.MouseLeftButtonDown -= AssociatedObjectOnMouseUp;
            }
            base.OnDetaching();
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (HistogramChartViewModel == null || e.ClickCount != 1 || !Keyboard.IsKeyDown(Key.LeftShift))
            {
                return;
            }

            HistogramChartViewModel.ToggleSelection(e.GetPosition(AssociatedObject));
        }
    }
}