namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Windows.Input;

    public class WheelsHistogramViewModel : AggregatedChartViewModel
    {
        private HistogramChartViewModel _frontLeftChartViewModel;
        private HistogramChartViewModel _frontRightChartViewModel;
        private HistogramChartViewModel _rearLeftChartViewModel;
        private HistogramChartViewModel _rearRightChartViewModel;
        private double _bandSize;
        private string _unit;
        private ICommand _refreshCommand;

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

        public double BandSize
        {
            get => _bandSize;
            set => SetProperty(ref _bandSize, value);
        }

        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        public ICommand RefreshCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }
    }
}