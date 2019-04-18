namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public abstract class AggregatedChartViewModel : AbstractViewModel, IAggregatedChartViewModel
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}