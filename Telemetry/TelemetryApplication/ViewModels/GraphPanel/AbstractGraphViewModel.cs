namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Windows.Media;
    using LiveCharts;
    using LiveCharts.Configurations;
    using LiveCharts.Geared;
    using LiveCharts.Wpf;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public abstract class AbstractGraphViewModel<T> : AbstractViewModel<LapTelemetryDto>, IGraphViewModel
    {
        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }

        protected override void ApplyModel(LapTelemetryDto model)
        {
            CartesianMapper<T> mapper = Mappers.Xy<T>().X(GetXValue).Y(GetYValue);
            _seriesCollection = new SeriesCollection(mapper);

            T[] dataPoints = GetDataPoints();
            LineSeries series = new LineSeries
            {
                Values = dataPoints.AsGearedValues().WithQuality(Quality.Low),
                Fill = Brushes.Transparent,
                StrokeThickness = 1,
                PointGeometry = null //use a null geometry when you have many series,
            };

            _seriesCollection.Add(series);
            NotifyPropertyChanged(nameof(SeriesCollection));

        }

        public override LapTelemetryDto SaveToNewModel()
        {
            return OriginalModel;
        }

        protected abstract T[] GetDataPoints();

        protected abstract double GetYValue(T value, int index);

        public abstract Func<double, string> YFormatter { get; }

        protected virtual double GetXValue(T value, int index) => OriginalModel.TimedTelemetrySnapshots[index].PlayerData.LapDistance;

        public virtual Func<double, string> XFormatter => d => d.ToString("N1");


    }
}