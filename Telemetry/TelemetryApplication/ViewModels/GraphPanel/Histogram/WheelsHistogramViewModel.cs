namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using SecondMonitor.ViewModels;

    public class WheelsHistogramViewModel : AbstractViewModel
    {
        private HistogramChartViewModel _leftFrontChartViewModel;
        private HistogramChartViewModel _rightFrontChartViewModel;
        private HistogramChartViewModel _leftRearChartViewModel;
        private HistogramChartViewModel _rightRearChartViewModel;
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public HistogramChartViewModel LeftFrontChartViewModel
        {
            get => _leftFrontChartViewModel;
            set => SetProperty(ref _leftFrontChartViewModel, value);
        }

        public HistogramChartViewModel RightFrontChartViewModel
        {
            get => _rightFrontChartViewModel;
            set => SetProperty(ref _rightFrontChartViewModel, value);
        }

        public HistogramChartViewModel LeftRearChartViewModel
        {
            get => _leftRearChartViewModel;
            set => SetProperty(ref _leftRearChartViewModel, value);
        }

        public HistogramChartViewModel RightRearChartViewModel
        {
            get => _rightRearChartViewModel;
            set => SetProperty(ref _rightRearChartViewModel, value);
        }
    }
}