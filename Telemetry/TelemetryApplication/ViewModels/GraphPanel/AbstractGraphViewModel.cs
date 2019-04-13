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
    using Settings.DTO;
    using TelemetryManagement.DTO;

    public abstract class AbstractGraphViewModel : AbstractViewModel, IGraphViewModel
    {
        private static readonly TimeSpan UpdateDelay = TimeSpan.FromMilliseconds(100);
        private LinearAxis _yAxis;
        private LinearAxis _xAxis;
        private PlotModel _plotModel;
        private Dictionary<string, (double x , Color color)> _selectedXValue;
        private bool _invalidatingPlot;
        private Task _invalidationTask;
        private double _yMaximum;
        private bool _updating;
        private bool _hasValidData;
        private ILapColorSynchronization _lapColorSynchronization;
        private IGraphViewSynchronization _graphViewSynchronization;
        private double _yMinimum;
        private bool _syncWithOtherGraphs;
        private DateTime _lastChangeRequest;
        private double _xMaximum;
        private double? _lapDistanceSector1;
        private double? _lapDistanceSector2;

        protected AbstractGraphViewModel()
        {
            SyncWithOtherGraphs = true;
            _lastChangeRequest = DateTime.MinValue;
            LoadedSeries = new Dictionary<string,(LapTelemetryDto telemetry, List<LineSeries> lineSeries)>();
            _selectedXValue = new Dictionary<string, (double x, Color color)>();
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

        protected Dictionary<string, (LapTelemetryDto telemetry, List<LineSeries> lineSeries)> LoadedSeries { get; }

        public bool HasValidData
        {
            get => _hasValidData;
            set => SetProperty(ref _hasValidData, value);
        }

        public XAxisKind XAxisKind { get; set; }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<string, (double x, Color color)> SelectedDistances
        {
            get => _selectedXValue;
            set => SetProperty(ref _selectedXValue, value);
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

        public DistanceUnits DistanceUnits { get; set; }
        public DistanceUnits SuspensionDistanceUnits { get; set; }
        public VelocityUnits VelocityUnits { get; set; }
        public VelocityUnits VelocityUnitsSmall { get; set; }
        public TemperatureUnits TemperatureUnits { get; set; }
        public PressureUnits PressureUnits { get; set; }
        public AngleUnits AngleUnits { get; set; }
        public ForceUnits ForceUnits { get; set; }

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

        protected double XMaximum
        {
            get => _xMaximum;
            set
            {
                _xMaximum = value;
                UpdateXAxis();
            }
        }



        public abstract string Title { get; }
        protected abstract string YUnits { get; }
        protected abstract double YTickInterval { get; }
        protected abstract bool CanYZoom { get; }


        public void AddLapTelemetry(LapTelemetryDto lapTelemetryDto)
        {
            Color color = GetLapColor(lapTelemetryDto.LapSummary);

            if (LoadedSeries.ContainsKey(lapTelemetryDto.LapSummary.Id))
            {
                RemoveLapTelemetry(lapTelemetryDto.LapSummary);
            }

            UpdateYMaximum(lapTelemetryDto);

            CheckAndCreateAxis();

            TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).ToArray();
            //TimedTelemetrySnapshot[] dataPoints = lapTelemetryDto.TimedTelemetrySnapshots.OrderBy(x => x.PlayerData.LapDistance).WhereWithPrevious(FilterFunction).ToArray();
            List<LineSeries> series = GetLineSeries(lapTelemetryDto.LapSummary, dataPoints, OxyColor.Parse(color.ToString()));

            if (series.Count == 0)
            {
                LoadedSeries.Add(lapTelemetryDto.LapSummary.Id, (lapTelemetryDto, series));
                return;
            }

            double maxX = 0.0;
            series.ForEach(x => maxX = Math.Max(maxX, x.Points.Max(y => y.X)));
            if (maxX > XMaximum)
            {
                XMaximum = maxX;
            }

            _selectedXValue[lapTelemetryDto.LapSummary.Id] = (0, color);
            LoadedSeries.Add(lapTelemetryDto.LapSummary.Id, (lapTelemetryDto,series));
            CheckIfHasValidData();
            InitializeSectorDistance(dataPoints);
            AddSectorGridLines();
            if (HasValidData)
            {
                series.ForEach(_plotModel.Series.Add);
                _plotModel.InvalidatePlot(true);
                NotifyPropertyChanged(nameof(PlotModel));
                NotifyPropertyChanged(nameof(PlotModel.Series));
                NotifyPropertyChanged(nameof(SelectedDistances));
            }
        }

        protected void RecreateAllLineSeries()
        {
            YMaximum = 0;
            YMinimum = 0;
            _yAxis?.Reset();
            InvalidatePlot();

            List<LapTelemetryDto> loadedLaps = LoadedSeries.Values.Select(x => x.telemetry).ToList();
            loadedLaps.ForEach(x => RemoveLapTelemetry(x.LapSummary));
            loadedLaps.ForEach(AddLapTelemetry);
        }

        private void InitializeSectorDistance(TimedTelemetrySnapshot[] dataPoints)
        {
            _lapDistanceSector1 = dataPoints.LastOrDefault(x => x.PlayerData.Timing.CurrentSector == 1)?.PlayerData.LapDistance;
            _lapDistanceSector2 = dataPoints.LastOrDefault(x => x.PlayerData.Timing.CurrentSector == 2)?.PlayerData.LapDistance;
        }

        private void AddSectorGridLines()
        {
            if (XAxisKind == XAxisKind.LapTime)
            {
                _xAxis.ExtraGridlines = new double[0];
            }

            if (!_lapDistanceSector1.HasValue || !_lapDistanceSector2.HasValue)
            {
                return;
            }

            double[] xValues = new [] {Distance.FromMeters(_lapDistanceSector1.Value).GetByUnit(DistanceUnits), Distance.FromMeters(_lapDistanceSector2.Value).GetByUnit(DistanceUnits)};
            _xAxis.ExtraGridlines = xValues;
        }

        private void CheckIfHasValidData()
        {
            bool hasValidData = false;
            foreach (List<LineSeries> loadedSeriesValue in LoadedSeries.Values.Select(x => x.lineSeries))
            {
                if (hasValidData)
                {
                    HasValidData = true;
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
                CreateYAxis();
            }

            if (_xAxis == null)
            {
                CreateXAxis();
            }
        }

        private void CreateYAxis()
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
                ExtraGridlines = new[] { 0.0 },
                ExtraGridlineThickness = 1.5,

            };
            if (YTickInterval > 0)
            {
                _yAxis.MajorStep = Math.Round(YTickInterval, 2);
                _yAxis.MajorGridlineStyle = LineStyle.Solid;
                _yAxis.MajorGridlineThickness = 1;
                _yAxis.MajorGridlineColor = OxyColor.Parse("#FF7F7F7F");
            }
            _plotModel.Axes.Add(_yAxis);
        }

        private void CreateXAxis()
        {
            _xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -1,
                Maximum = 0,
                TickStyle = TickStyle.Inside,
                AxislineColor = OxyColor.Parse("#FFD6D6D6"),
                MajorStep = XAxisKind == XAxisKind.LapTime ? 20 : 200,
                MajorGridlineColor = OxyColor.Parse("#464239"),
                MajorGridlineStyle = LineStyle.LongDash,
                Unit = XAxisKind == XAxisKind.LapTime ? "s" : Distance.GetUnitsSymbol(DistanceUnits),
                AxisTitleDistance = 0,
                AxisDistance = 0,
                ExtraGridlineColor = OxyColors.Red,
                ExtraGridlineThickness = 1,
            };

            _plotModel.Axes.Add(_xAxis);
            _xAxis.AxisChanged += XAxisOnAxisChanged;
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
            if (LoadedSeries.TryGetValue(lapSummaryDto.Id, out(LapTelemetryDto telemetry, List<LineSeries> lineSeries) value))
            {
                LoadedSeries.Remove(lapSummaryDto.Id);
                value.lineSeries.ForEach(x => _plotModel.Series.Remove(x));
                InvalidatePlot();
            }

            _selectedXValue.Remove(lapSummaryDto.Id);
            NotifyPropertyChanged(nameof(SelectedDistances));
            if (LoadedSeries.Count == 0)
            {
                XMaximum = 0;
            }
        }

        public void UpdateXSelection(string lapId, TimedTelemetrySnapshot timedTelemetrySnapshot)
        {
            if(_selectedXValue.TryGetValue(lapId, out (double x, Color color) value))
            {
                value.x = GetXValue(timedTelemetrySnapshot);
                _selectedXValue[lapId] = value;
            }
            NotifyPropertyChanged(nameof(SelectedDistances));
        }

        protected double GetXValue(TimedTelemetrySnapshot timedTelemetrySnapshot)
        {
            switch (XAxisKind)
            {
                case XAxisKind.LapDistance:
                    return Distance.FromMeters(timedTelemetrySnapshot.PlayerData.LapDistance).GetByUnit(DistanceUnits);
                case XAxisKind.LapTime:
                    return timedTelemetrySnapshot.LapTimeSeconds;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void InvalidatePlot()
        {
            _invalidationTask = InvalidatePlotAsync();
        }

        protected async Task InvalidatePlotAsync()
        {
            _lastChangeRequest = DateTime.Now;
            if (_invalidatingPlot && !HasValidData)
            {
                return;
            }

            _invalidatingPlot = true;
            while (DateTime.Now - _lastChangeRequest < UpdateDelay)
            {
                await Task.Delay(UpdateDelay).ConfigureAwait(false);
            }

            _plotModel.PlotView.InvalidatePlot(true);
            _invalidatingPlot = false;
        }

        protected abstract void UpdateYMaximum(LapTelemetryDto lapTelemetry);

        private void UpdateYAxis()
        {
            if (_yAxis == null)
            {
                return;
            }

            _yAxis.Maximum = YMaximum;
            _yAxis.Minimum = YMinimum;
            InvalidatePlot();
        }

        private void UpdateXAxis()
        {
            if (_xAxis == null)
            {
                return;
            }

            _xAxis.Maximum = XMaximum;
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
            if (_selectedXValue.TryGetValue(e.LapId, out (double x, Color color) value))
            {
                value.color = e.Color;
                _selectedXValue[e.LapId] = value;
            }

            if (LoadedSeries.TryGetValue(e.LapId, out (LapTelemetryDto telemetry, List<LineSeries> lineSeries) series))
            {
                OxyColor color = OxyColor.Parse(e.Color.ToString());
                ApplyNewLineColor(series.lineSeries, color);
            }

            NotifyPropertyChanged(nameof(SelectedDistances));
            InvalidatePlot();
        }

        protected virtual void ApplyNewLineColor(List<LineSeries> series, OxyColor newColor)
        {
            foreach (LineSeries lineSeries in series)
            {
                lineSeries.Color = newColor;
                lineSeries.TextColor = newColor;
            }
        }

        private Color GetLapColor(LapSummaryDto lapSummaryDto)
        {
            if (LapColorSynchronization == null || !LapColorSynchronization.TryGetColorForLap(lapSummaryDto.Id, out Color color))
            {
                color = Colors.Red;
            }

            return color;
        }

        protected abstract List<LineSeries> GetLineSeries(LapSummaryDto lapSummary, TimedTelemetrySnapshot[] dataPoints, OxyColor color);

        protected LineSeries CreateLineSeries(string title, OxyColor color, LineStyle lineStyle = LineStyle.Solid)
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
                TrackerFormatString = "{0}\n"+ (XAxisKind == XAxisKind.LapTime ? "s" : Distance.GetUnitsSymbol(DistanceUnits)) + ": {2}\n" +YUnits +": {4}"
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