namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataExtractor;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Series;
    using TelemetryManagement.DTO;

    public abstract class AbstractSingleSeriesGraphViewModel : AbstractGraphViewModel
    {
        private ISingleSeriesDataExtractor _selectedDataExtractor;

        protected AbstractSingleSeriesGraphViewModel(IEnumerable<ISingleSeriesDataExtractor> dataExtractors)
        {
            DataExtractors = dataExtractors.ToList();
            SelectedDataExtractor = DataExtractors.First();
        }

        public IReadOnlyCollection<ISingleSeriesDataExtractor> DataExtractors { get; }

        public ISingleSeriesDataExtractor SelectedDataExtractor
        {
            get => _selectedDataExtractor;
            set
            {
                UnSubscribeDataExtractor();
                SetProperty(ref _selectedDataExtractor, value);
                SubscribeDataExtractor();
                RecreateAllLineSeries();
            }
        }

        protected override List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color)
        {
            if (!_selectedDataExtractor.ShowLapGraph(lapSummary))
            {
                return new List<LineSeries>();
            }

            LineSeries newLineSeries = CreateLineSeries($"Lap {lapSummary.CustomDisplayName}", color);
            List<DataPoint> plotDataPoints = dataPoints.Select(x => new DataPoint(GetXValue(x), _selectedDataExtractor.GetValue(GetYValue, x, XAxisKind))).ToList();

            newLineSeries.Points.AddRange(plotDataPoints);

            List<LineSeries> series = new List<LineSeries>(1) {newLineSeries};
            return series;
        }

        protected abstract double GetYValue(TimedTelemetrySnapshot value);

        private void SubscribeDataExtractor()
        {
            if (_selectedDataExtractor == null)
            {
                return;
            }

            _selectedDataExtractor.DataRefreshRequested += SelectedDataExtractorOnDataRefreshRequested;
        }

        private void UnSubscribeDataExtractor()
        {
            if (_selectedDataExtractor == null)
            {
                return;
            }

            _selectedDataExtractor.DataRefreshRequested -= SelectedDataExtractorOnDataRefreshRequested;
        }

        private void SelectedDataExtractorOnDataRefreshRequested(object sender, EventArgs e)
        {
            RecreateAllLineSeries();
        }


        public override void Dispose()
        {
            UnSubscribeDataExtractor();
            DataExtractors.ForEach(x => x.Dispose());
            base.Dispose();
        }
    }


}