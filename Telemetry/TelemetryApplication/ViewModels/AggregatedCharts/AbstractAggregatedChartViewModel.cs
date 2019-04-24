namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using SecondMonitor.ViewModels;

    public abstract class AbstractAggregatedChartViewModel : AbstractViewModel, IAggregatedChartViewModel
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public abstract void Dispose();
    }
}