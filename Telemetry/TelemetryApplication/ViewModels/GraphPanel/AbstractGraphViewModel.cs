namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using Controllers.Synchronization;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using LiveCharts;
    using LiveCharts.Configurations;
    using LiveCharts.Geared;
    using LiveCharts.Wpf;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public abstract class AbstractGraphViewModel : AbstractViewModel, IGraphViewModel
    {
        private SeriesCollection _seriesCollection;
        private readonly Dictionary<string, LineSeries> _loadedSeries;

        protected AbstractGraphViewModel()
        {
            _loadedSeries = new Dictionary<string, LineSeries>();
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }

        public ILapColorSynchronization LapColorSynchronization { get; set; }

        public void AddLapTelemetry(LapTelemetryDto lapTelemetryDto)
        {
            if (LapColorSynchronization == null || !LapColorSynchronization.TryGetColorForLap(lapTelemetryDto.LapSummary.Id, out Color color))
            {
                color = Colors.Red;
            }

            if (_seriesCollection == null)
            {
                CartesianMapper<TimedTelemetrySnapshot> mapper = Mappers.Xy<TimedTelemetrySnapshot>().X(GetXValue).Y(GetYValue);
                _seriesCollection = new SeriesCollection(mapper);
            }

            TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).WhereWithPrevious(FilterFunction).ToArray();

            LineSeries series = new LineSeries
            {
                Values = dataPoints.AsGearedValues().WithQuality(Quality.Low),
                Fill = Brushes.Transparent,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(color),
                LineSmoothness = 0,
                PointGeometry = null //use a null geometry when you have many series,
            };

            _loadedSeries.Add(lapTelemetryDto.LapSummary.Id, series);

            _seriesCollection.Add(series);
            NotifyPropertyChanged(nameof(SeriesCollection));
        }

        public void RemoveLapTelemetry(LapSummaryDto lapSummaryDto)
        {
            if (_loadedSeries.TryGetValue(lapSummaryDto.Id, out LineSeries lineSeries))
            {
                _seriesCollection.Remove(lineSeries);
                _loadedSeries.Remove(lapSummaryDto.Id);
                NotifyPropertyChanged(nameof(SeriesCollection));
            }
        }

        protected virtual bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot) => currentSnapshot.LapTimeSeconds - previousSnapshot.LapTimeSeconds > 1;

        protected abstract double GetYValue(TimedTelemetrySnapshot value, int index);

        public virtual Func<double, string> YFormatter => d => d.ToString("N1");

        protected abstract double GetXValue(TimedTelemetrySnapshot value, int index);

        public virtual Func<double, string> XFormatter => d => d.ToString("N1");


    }
}