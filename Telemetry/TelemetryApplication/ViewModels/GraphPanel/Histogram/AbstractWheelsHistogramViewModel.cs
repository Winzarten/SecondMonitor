namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Windows.Input;
    using AggregatedCharts;

    public abstract class AbstractWheelsHistogramViewModel<T> : AggregatedChartViewModel  where T : HistogramChartViewModel, new()
    {
        private T _frontLeftChartViewModel;
        private T _frontRightChartViewModel;
        private T _rearLeftChartViewModel;
        private T _rearRightChartViewModel;
        private double _bandSize;
        private string _unit;
        private ICommand _refreshCommand;

        protected AbstractWheelsHistogramViewModel()
        {
            FrontLeftChartViewModel = new T();
            FrontRightChartViewModel = new T();
            RearLeftChartViewModel = new T();
            RearRightChartViewModel = new T();
        }

        public T FrontLeftChartViewModel
        {
            get => _frontLeftChartViewModel;
            set => SetProperty(ref _frontLeftChartViewModel, value);
        }

        public T FrontRightChartViewModel
        {
            get => _frontRightChartViewModel;
            set => SetProperty(ref _frontRightChartViewModel, value);
        }

        public T RearLeftChartViewModel
        {
            get => _rearLeftChartViewModel;
            set => SetProperty(ref _rearLeftChartViewModel, value);
        }

        public T RearRightChartViewModel
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