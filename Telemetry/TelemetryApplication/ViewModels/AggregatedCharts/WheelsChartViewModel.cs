namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public class WheelsChartViewModel : AggregatedChartViewModel
    {
        private IAggregatedChartViewModel  _frontLeftChartViewModel;
        private IAggregatedChartViewModel _frontRightChartViewModel;
        private IAggregatedChartViewModel _rearLeftChartViewModel;
        private IAggregatedChartViewModel _rearRightChartViewModel;


        public IAggregatedChartViewModel FrontLeftChartViewModel
        {
            get => _frontLeftChartViewModel;
            set => SetProperty(ref _frontLeftChartViewModel, value);
        }

        public IAggregatedChartViewModel FrontRightChartViewModel
        {
            get => _frontRightChartViewModel;
            set => SetProperty(ref _frontRightChartViewModel, value);
        }

        public IAggregatedChartViewModel RearLeftChartViewModel
        {
            get => _rearLeftChartViewModel;
            set => SetProperty(ref _rearLeftChartViewModel, value);
        }

        public IAggregatedChartViewModel RearRightChartViewModel
        {
            get => _rearRightChartViewModel;
            set => SetProperty(ref _rearRightChartViewModel, value);
        }
    }
}