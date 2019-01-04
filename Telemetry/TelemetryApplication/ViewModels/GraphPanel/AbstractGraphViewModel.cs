namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Controllers.Synchronization;
    using Controllers.Synchronization.Graphs;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public abstract class AbstractGraphViewModel : AbstractViewModel, IGraphViewModel
    {
        private readonly Dictionary<string, List<LineSeries>> _loadedSeries;
        private LinearAxis _yAxis;
        private LinearAxis _xAxis;
        private PlotModel _plotModel;
        private Distance _selectedDistance;
        private bool _invalidatingPlot;
        private Task _invalidationTask;
        private double _yMaximum;
        private bool _updating;
        private ILapColorSynchronization _lapColorSynchronization;
        private IGraphViewSynchronization _graphViewSynchronization;
        private double _yMinimum;

        protected AbstractGraphViewModel()
        {
            _loadedSeries = new Dictionary<string, List<LineSeries>>();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _plotModel = new PlotModel()
            {
                Title = Title,
                TextColor = OxyColor.Parse("#FFD6D6D6"),
                LegendPlacement = LegendPlacement.Outside
            };


        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }

        public Distance SelectedDistance
        {
            get => _selectedDistance;
            set
            {
                _selectedDistance = value;
                UpdateSelectedDistanceAsync();
            }
        }
        public ILapColorSynchronization LapColorSynchronization { get => _lapColorSynchronization;
            set
            {
                UnsubscribeLapColorSync();
                _lapColorSynchronization = value;
                SubscribeLapColorSync();
            } }

        public IGraphViewSynchronization GraphViewSynchronization
        {
            get => _graphViewSynchronization;
            set
            {
                UnsubscribeGraphViewSync();
                _graphViewSynchronization = value;
                SubscribeGraphViewSync();
            }
        }



        public Distance TrackDistance { get; set; }

        public DistanceUnits DistanceUnits { get; set; }
        public VelocityUnits VelocityUnits { get; set; }

        protected double YMaximum
        {
            get => _yMaximum;
            set
            {
                if (_yMaximum == value)
                {
                    return;
                }

                _yMaximum = value;
                UpdateYAxis();
            }
        }

        protected double YMinimum
        {
            get => _yMinimum;
            set
            {
                if (_yMinimum == value)
                {
                    return;
                }

                _yMinimum = value;
                UpdateYAxis();
            }
        }

        protected double XMaximum => TrackDistance.GetByUnit(DistanceUnits);

        public abstract string Title { get; }
        protected abstract string YUnits { get; }
        protected abstract double YTickInterval { get; }
        protected abstract bool CanYZooM { get; }


        public void AddLapTelemetry(LapTelemetryDto lapTelemetryDto)
        {

            if (LapColorSynchronization == null || !LapColorSynchronization.TryGetColorForLap(lapTelemetryDto.LapSummary.Id, out Color color))
            {
                color = Colors.Red;
            }

            UpdateYMaximum(lapTelemetryDto);

            CheckAndCreateAxis();

            TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).ToArray();
            //TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).WhereWithPrevious(FilterFunction).ToArray();
            List<LineSeries> series = GetLineSeries(lapTelemetryDto.LapSummary, dataPoints, OxyColor.Parse(color.ToString()));

            series.ForEach(_plotModel.Series.Add);
            _plotModel.InvalidatePlot(true);

            _loadedSeries.Add(lapTelemetryDto.LapSummary.Id, series);

            NotifyPropertyChanged(nameof(PlotModel));
            NotifyPropertyChanged(nameof(PlotModel.Series));
        }

        private void CheckAndCreateAxis()
        {
            if (_yAxis == null)
            {
                _yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = YMinimum,
                    Maximum = YMaximum,
                    TickStyle = TickStyle.Inside,
                    AxislineColor = OxyColor.Parse("#FFD6D6D6"),
                    IsZoomEnabled = CanYZooM,
                    IsPanEnabled = CanYZooM,
                    Unit = YUnits,
                };
                if (YTickInterval > 0)
                {
                    _yAxis.MajorStep = YTickInterval;
                    _yAxis.MajorGridlineStyle = LineStyle.Solid;
                    _yAxis.MajorGridlineThickness = 1;
                    _yAxis.MajorGridlineColor = OxyColor.Parse("#FF7F7F7F");
                }

                _plotModel.Axes.Add(_yAxis);
            }

            if (_xAxis == null)
            {
                _xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -1,
                    Maximum = XMaximum,
                    TickStyle = TickStyle.Inside,
                    AxislineColor = OxyColor.Parse("#FFD6D6D6"),
                    ExtraGridlineColor = OxyColors.DarkRed, //OxyColor.Parse("#FFD6D6D6"),
                    ExtraGridlineThickness = 2,
                    MajorStep = 200,
                    MajorGridlineColor = OxyColor.Parse("#464239"),
                    MajorGridlineStyle = LineStyle.LongDash,
                    Unit = Distance.GetUnitsSymbol(DistanceUnits),
                };

                _plotModel.Axes.Add(_xAxis);
                _xAxis.AxisChanged += XAxisOnAxisChanged;
            }
        }

        private void XAxisOnAxisChanged(object sender, AxisChangedEventArgs e)
        {
            if (!_updating)
            {
                _graphViewSynchronization.NotifyPanChanged(this, _xAxis.ActualMinimum, _xAxis.ActualMaximum);
            }
        }


        public void UpdateSelectedDistanceAsync()
        {
            _xAxis.ExtraGridlines = new[] {SelectedDistance.InMeters};
            InvalidatePlot();

        }

        public void RemoveLapTelemetry(LapSummaryDto lapSummaryDto)
        {
            if (_loadedSeries.TryGetValue(lapSummaryDto.Id, out List<LineSeries> lineSeries))
            {
                _loadedSeries.Remove(lapSummaryDto.Id);
                lineSeries.ForEach(x => _plotModel.Series.Remove(x));
                InvalidatePlot();
            }
        }

        protected void InvalidatePlot()
        {
            _invalidationTask = InvalidatePlotAsync();
        }

        protected async Task InvalidatePlotAsync()
        {
            if (_invalidatingPlot)
            {
                return;
            }

            _invalidatingPlot = true;
            await Task.Delay(1000).ConfigureAwait(false);
            _plotModel.PlotView.InvalidatePlot(true);
            _invalidatingPlot = false;
        }

        protected abstract void UpdateYMaximum(LapTelemetryDto lapTelemetry);

        private void UpdateYAxis()
        {
            if (_xAxis == null)
            {
                return;
            }

            _yAxis.Maximum = YMaximum;
            _yAxis.Minimum = YMinimum;
            InvalidatePlot();
        }

        private void SubscribeGraphViewSync()
        {
            if (_graphViewSynchronization == null)
            {
                return;
            }

            _graphViewSynchronization.ScaleChanged += GraphViewSynchronizationOnScaleChanged;
            _graphViewSynchronization.PanChanged += GraphViewSynchronizationOnPanChanged;
        }

        private void GraphViewSynchronizationOnPanChanged(object sender, PanEventArgs e)
        {
            if (ReferenceEquals(sender, this) || _xAxis == null)
            {
                return;
            }
            _updating = true;
            _xAxis.Minimum = e.Minimum;
            _xAxis.Maximum = e.Maximum;
            _xAxis.Reset();
            InvalidatePlot();
            _updating = false;
        }

        private void GraphViewSynchronizationOnScaleChanged(object sender, ScaleEventArgs e)
        {
            if (ReferenceEquals(sender, this) || _xAxis == null)
            {
                return;
            }

            _updating = true;
            _xAxis.Zoom(e.NewScale);
            InvalidatePlot();
            _updating = false;
        }

        private void UnsubscribeGraphViewSync()
        {
            if (_graphViewSynchronization == null)
            {
                return;
            }

            _graphViewSynchronization.ScaleChanged -= GraphViewSynchronizationOnScaleChanged;
            _graphViewSynchronization.PanChanged -= GraphViewSynchronizationOnPanChanged;
        }

        private void UnsubscribeLapColorSync()
        {
            if (_lapColorSynchronization == null)
            {
                return;
            }

            _lapColorSynchronization.LapColorChanged -= LapColorSynchronizationOnLapColorChanged;
        }

        private void SubscribeLapColorSync()
        {
            if (_lapColorSynchronization == null)
            {
                return;
            }

            _lapColorSynchronization.LapColorChanged += LapColorSynchronizationOnLapColorChanged;
        }

        private void LapColorSynchronizationOnLapColorChanged(object sender, LapColorArgs e)
        {
            if (_loadedSeries.TryGetValue(e.LapId, out List<LineSeries> series))
            {
                OxyColor color = OxyColor.Parse(e.Color.ToString());
                foreach (LineSeries lineSeries in series)
                {
                    lineSeries.Color = color;
                    lineSeries.TextColor = color;
                }
            }
            InvalidatePlot();
        }

        protected double GetXValue(TimedTelemetrySnapshot value)
        {
            return Distance.FromMeters(value.PlayerData.LapDistance).GetByUnit(DistanceUnits);
        }

        protected abstract List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color);

        //protected abstract bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot);

        protected abstract double GetYValue(TimedTelemetrySnapshot value);

        public void Dispose()
        {
            _invalidationTask?.Dispose();
            UnsubscribeLapColorSync();
            UnsubscribeGraphViewSync();

            if (_xAxis != null)
            {
                _xAxis.AxisChanged -= XAxisOnAxisChanged;
            }
        }
    }
}