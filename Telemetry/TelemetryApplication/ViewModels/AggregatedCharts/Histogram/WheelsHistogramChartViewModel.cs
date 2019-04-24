namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts.Histogram
{
    using System.Windows.Input;

    public class WheelsHistogramChartViewModel : WheelsChartViewModel
    {
        private double _bandSize;
        private string _unit;
        private ICommand _refreshCommand;

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