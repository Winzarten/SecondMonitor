namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using SecondMonitor.ViewModels;

    public class AggregatedChartSelectorViewModel : AbstractViewModel, IAggregatedChartSelectorViewModel
    {
        private IReadOnlyCollection<string> _histogramChartNames;
        private string _selectedHistogramChartName;
        private IReadOnlyCollection<string> _plotChartNames;
        private string _selectedPlotChartName;
        private int _selectedTabIndex;
        private ICommand _openSelectedChartCommand;
        private ICommand _cancelAndCloseWindowCommand;

        public IReadOnlyCollection<string> HistogramChartNames
        {
            get => _histogramChartNames;
            set => SetProperty(ref _histogramChartNames, value);
        }

        public string SelectedHistogramChartName
        {
            get => _selectedHistogramChartName;
            set => SetProperty(ref _selectedHistogramChartName, value);
        }

        public IReadOnlyCollection<string> ScatterPlotChartNames
        {
            get => _plotChartNames;
            set => SetProperty(ref _plotChartNames, value);
        }

        public string SelectedScatterPlotChartName
        {
            get => _selectedPlotChartName;
            set => SetProperty(ref _selectedPlotChartName, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public ICommand OpenSelectedChartCommand
        {
            get => _openSelectedChartCommand;
            set => SetProperty(ref _openSelectedChartCommand, value);
        }

        public ICommand CancelAndCloseWindowCommand
        {
            get => _cancelAndCloseWindowCommand;
            set => SetProperty(ref _cancelAndCloseWindowCommand, value);
        }
    }
}