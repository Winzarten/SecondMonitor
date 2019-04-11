namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using SecondMonitor.ViewModels;

    public abstract class AggregatedChartViewModel : AbstractViewModel
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}