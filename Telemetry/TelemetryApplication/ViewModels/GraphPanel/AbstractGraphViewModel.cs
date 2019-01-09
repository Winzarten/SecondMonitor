namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
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
        private LinearAxis _yAxis;
        private LinearAxis _xAxis;
        private PlotModel _plotModel;
        private Dictionary<string, (Distance distance , Color color)> _selectedDistances;
        private bool _invalidatingPlot;
        private Task _invalidationTask;
        private double _yMaximum;
        private bool _updating;
        private bool _hasValidData;
        private ILapColorSynchronization _lapColorSynchronization;
        private IGraphViewSynchronization _graphViewSynchronization;
        private double _yMinimum;
        private bool _syncWithOtherGraphs;

        protected AbstractGraphViewModel()
        {
            SyncWithOtherGraphs = true;
            LoadedSeries = new Dictionary<string, List<LineSeries>>();
            _selectedDistances = new Dictionary<string, (Distance distance, Color color)>();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _plotModel = new PlotModel()
            {
                Title = Title,
                TitlePadding = 0,
                LegendBorderThickness = 1,
                LegendBorder = OxyColor.Parse("#FFD6D6D6"),
                TextColor = OxyColor.Parse("#FFD6D6D6"),
                LegendPlacement = LegendPlacement.Outside,
                TitleFontSize = 12,

            };
        }

        protected Dictionary<string, List<LineSeries>> LoadedSeries { get; }

        public bool HasValidData
        {
            get => _hasValidData;
            set => SetProperty(ref _hasValidData, value);
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

        public Dictionary<string, (Distance distance, Color color)> SelectedDistances
        {
            get => _selectedDistances;
            set => SetProperty(ref _selectedDistances, value);
        }

        public bool SyncWithOtherGraphs
        {
            get => _syncWithOtherGraphs;
            set => SetProperty(ref _syncWithOtherGraphs, value);
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
        public DistanceUnits SuspensionDistanceUnits { get; set; }
        public VelocityUnits VelocityUnits { get; set; }
        public TemperatureUnits TemperatureUnits { get; set; }
        public PressureUnits PressureUnits { get; set; }

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
        protected abstract bool CanYZoom { get; }


        public void AddLapTelemetry(LapTelemetryDto lapTelemetryDto)
        {
            if (LapColorSynchronization == null || !LapColorSynchronization.TryGetColorForLap(lapTelemetryDto.LapSummary.Id, out Color color))
            {
                color = Colors.Red;
            }

            if (LoadedSeries.ContainsKey(lapTelemetryDto.LapSummary.Id))
            {
                RemoveLapTelemetry(lapTelemetryDto.LapSummary);
            }

            UpdateYMaximum(lapTelemetryDto);

            CheckAndCreateAxis();

            TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).ToArray();
            //TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).WhereWithPrevious(FilterFunction).ToArray();
            List<LineSeries> series = GetLineSeries(lapTelemetryDto.LapSummary, dataPoints, OxyColor.Parse(color.ToString()));

            _selectedDistances[lapTelemetryDto.LapSummary.Id] = (Distance.ZeroDistance, color);
            LoadedSeries.Add(lapTelemetryDto.LapSummary.Id, series);
            CheckIfHasValidData();

            if (HasValidData)
            {
                series.ForEach(_plotModel.Series.Add);
                _plotModel.InvalidatePlot(true);
                NotifyPropertyChanged(nameof(PlotModel));
                NotifyPropertyChanged(nameof(PlotModel.Series));
                NotifyPropertyChanged(nameof(SelectedDistances));
            }
        }

        private void CheckIfHasValidData()
        {
            bool hasValidData = false;
            foreach (List<LineSeries> loadedSeriesValue in LoadedSeries.Values)
            {
                if (hasValidData)
                {
                    HasValidData = hasValidData;
                    return;
                }

                loadedSeriesValue.ForEach(x => hasValidData = hasValidData || x.Points.Any(y => y.Y != x.Points.First().Y));
            }

            HasValidData = hasValidData;
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
                    IsZoomEnabled = CanYZoom,
                    IsPanEnabled = CanYZoom,
                    Unit = YUnits,
                    AxisTitleDistance = 0,
                    AxisDistance = 0,
                    ExtraGridlineColor = OxyColors.Red,
                    ExtraGridlines = new [] {0.0},
                    ExtraGridlineThickness = 1.5

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
                    MajorStep = 200,
                    MajorGridlineColor = OxyColor.Parse("#464239"),
                    MajorGridlineStyle = LineStyle.LongDash,
                    Unit = Distance.GetUnitsSymbol(DistanceUnits),
                    AxisTitleDistance = 0,
                    AxisDistance = 0,
                };

                _plotModel.Axes.Add(_xAxis);
                _xAxis.AxisChanged += XAxisOnAxisChanged;
            }
        }

        private void XAxisOnAxisChanged(object sender, AxisChangedEventArgs e)
        {
            if (!_updating && _syncWithOtherGraphs)
            {
                _graphViewSynchronization.NotifyPanChanged(this, _xAxis.ActualMinimum, _xAxis.ActualMaximum);
            }
        }


        public void RemoveLapTelemetry(LapSummaryDto lapSummaryDto)
        {
            if (LoadedSeries.TryGetValue(lapSummaryDto.Id, out List<LineSeries> lineSeries))
            {
                LoadedSeries.Remove(lapSummaryDto.Id);
                lineSeries.ForEach(x => _plotModel.Series.Remove(x));
                InvalidatePlot();
            }

            _selectedDistances.Remove(lapSummaryDto.Id);
            NotifyPropertyChanged(nameof(SelectedDistances));
        }

        public void UpdateLapDistance(string lapId, Distance distance)
        {
            if(_selectedDistances.TryGetValue(lapId, out (Distance distance, Color color) value))
            {
                value.distance = distance;
                _selectedDistances[lapId] = value;
            }
            NotifyPropertyChanged(nameof(SelectedDistances));
        }

        protected void InvalidatePlot()
        {
            _invalidationTask = InvalidatePlotAsync();
        }

        protected async Task InvalidatePlotAsync()
        {
            if (_invalidatingPlot && !HasValidData)
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

        protected virtual void SubscribeGraphViewSync()
        {
            if (_graphViewSynchronization == null)
            {
                return;
            }

            _graphViewSynchronization.PanChanged += GraphViewSynchronizationOnPanChanged;
        }

        private void GraphViewSynchronizationOnPanChanged(object sender, PanEventArgs e)
        {
            if (!SyncWithOtherGraphs || ReferenceEquals(sender, this) || _xAxis == null)
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

        protected virtual void UnsubscribeGraphViewSync()
        {
            if (_graphViewSynchronization == null)
            {
                return;
            }

            _graphViewSynchronization.PanChanged -= GraphViewSynchronizationOnPanChanged;
        }

        protected virtual void UnsubscribeLapColorSync()
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
            if (_selectedDistances.TryGetValue(e.LapId, out (Distance distance, Color color) value))
            {
                value.color = e.Color;
                _selectedDistances[e.LapId] = value;
            }

            if (LoadedSeries.TryGetValue(e.LapId, out List<LineSeries> series))
            {
                OxyColor color = OxyColor.Parse(e.Color.ToString());
                foreach (LineSeries lineSeries in series)
                {
                    lineSeries.Color = color;
                    lineSeries.TextColor = color;
                }
            }

            NotifyPropertyChanged(nameof(SelectedDistances));
            InvalidatePlot();
        }

        protected double GetXValue(TimedTelemetrySnapshot value)
        {
            return Distance.FromMeters(value.PlayerData.LapDistance).GetByUnit(DistanceUnits);
        }

        protected abstract List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color);

        protected static LineSeries CreateLineSeries(string title, OxyColor color, LineStyle lineStyle = LineStyle.Solid)
        {
            return new LineSeries
            {
                Title = title,
                Color = color,
                TextColor = color,
                InterpolationAlgorithm = null,
                CanTrackerInterpolatePoints = false,
                StrokeThickness = 2,
                LineStyle = lineStyle,
            };
        }

        //protected abstract bool FilterFunction(TimedTelemetrySnapshot previousSnapshot, TimedTelemetrySnapshot currentSnapshot);

        public void Dispose()
        {
            UnsubscribeLapColorSync();
            UnsubscribeGraphViewSync();

            if (_xAxis != null)
            {
                _xAxis.AxisChanged -= XAxisOnAxisChanged;
            }
        }
    }
}