namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public interface IAggregatedChartSelectorViewModel : IViewModel
    {
        IReadOnlyCollection<string> HistogramChartNames { get; set; }
        string SelectedHistogramChartName { get; set; }

        IReadOnlyCollection<string> ScatterPlotChartNames { get; set; }
        string SelectedScatterPlotChartName { get; set; }

        int SelectedTabIndex { get; set; }
        ICommand OpenSelectedChartCommand { get; set; }
        ICommand CancelAndCloseWindowCommand { get; set; }
    }
}
