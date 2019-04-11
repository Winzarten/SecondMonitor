namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    public class WheelsHistogramViewModel : AggregatedChartViewModel
    {
        private HistogramChartViewModel _frontLeftChartViewModel;
        private HistogramChartViewModel _frontRightChartViewModel;
        private HistogramChartViewModel _rearLeftChartViewModel;
        private HistogramChartViewModel _rearRightChartViewModel;

        public WheelsHistogramViewModel()
        {
            FrontLeftChartViewModel = new HistogramChartViewModel();
            FrontRightChartViewModel = new HistogramChartViewModel();
            RearLeftChartViewModel = new HistogramChartViewModel();
            RearRightChartViewModel = new HistogramChartViewModel();
        }

        public HistogramChartViewModel FrontLeftChartViewModel
        {
            get => _frontLeftChartViewModel;
            set => SetProperty(ref _frontLeftChartViewModel, value);
        }

        public HistogramChartViewModel FrontRightChartViewModel
        {
            get => _frontRightChartViewModel;
            set => SetProperty(ref _frontRightChartViewModel, value);
        }

        public HistogramChartViewModel RearLeftChartViewModel
        {
            get => _rearLeftChartViewModel;
            set => SetProperty(ref _rearLeftChartViewModel, value);
        }

        public HistogramChartViewModel RearRightChartViewModel
        {
            get => _rearRightChartViewModel;
            set => SetProperty(ref _rearRightChartViewModel, value);
        }
    }
}