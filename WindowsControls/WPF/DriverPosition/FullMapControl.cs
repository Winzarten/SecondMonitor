namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.TrackMap;

    public class FullMapControl : AbstractSituationOverviewControl
    {
        private readonly double _topOffset;
        private readonly double _leftOffset;
        private readonly double _canvasWidth;
        private readonly double _canvasHeight;
        private readonly string _fullMapGeometry;
        private readonly string _finnishLineGeometry;
        private readonly string _sector1Geometry;
        private readonly string _sector2Geometry;
        private readonly string _sector3Geometry;

        private MapSidePanelControl _mapSidePanelControl;
        private Canvas _mainCanvas;
        private bool _autoScaleDriverControls;
        private Viewbox _viewbox;
        private Path _sector1Path;
        private Path _sector2Path;
        private Path _sector3Path;

        public static readonly DependencyProperty DriverControllerSizeProperty = DependencyProperty.Register("DriverControllerSize", typeof(double), typeof(FullMapControl));
        public static readonly DependencyProperty DriverControllerFontSizeProperty = DependencyProperty.Register("DriverControllerFontSize", typeof(double), typeof(FullMapControl));

        public static readonly DependencyProperty YellowSectorBrushProperty = DependencyProperty.Register("YellowSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));
        public static readonly DependencyProperty GreenSectorBrushProperty = DependencyProperty.Register("GreenSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));
        public static readonly DependencyProperty PurpleSectorBrushProperty = DependencyProperty.Register("PurpleSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));

        public FullMapControl(ITrackMap trackMap)
        {
            _topOffset = trackMap.TrackGeometry.TopOffset;
            _leftOffset = trackMap.TrackGeometry.LeftOffset;
            _canvasWidth = trackMap.TrackGeometry.Width;
            _canvasHeight = trackMap.TrackGeometry.Height;
            _fullMapGeometry = trackMap.TrackGeometry.FullMapGeometry;
            _finnishLineGeometry = trackMap.TrackGeometry.StartLineGeometry;
            _sector1Geometry = trackMap.TrackGeometry.Sector1Geometry;
            _sector2Geometry = trackMap.TrackGeometry.Sector2Geometry;
            _sector3Geometry = trackMap.TrackGeometry.Sector3Geometry;
            RefreshDriverControllerSize();
            InitializeMap();
        }

        public SolidColorBrush PurpleSectorBrush
        {
            get => (SolidColorBrush)GetValue(PurpleSectorBrushProperty);
            set => SetValue(PurpleSectorBrushProperty, value);
        }

        public SolidColorBrush GreenSectorBrush
        {
            get => (SolidColorBrush)GetValue(GreenSectorBrushProperty);
            set => SetValue(GreenSectorBrushProperty, value);
        }

        public SolidColorBrush YellowSectorBrush
        {
            get => (SolidColorBrush)GetValue(YellowSectorBrushProperty);
            set => SetValue(YellowSectorBrushProperty, value);
        }


        public double DriverControllerFontSize
        {
            get => (double)GetValue(DriverControllerFontSizeProperty);
            set => SetValue(DriverControllerFontSizeProperty, value);
        }

        public bool AutoScaleDriverControls
        {
            get => _autoScaleDriverControls;
            set
            {
                _autoScaleDriverControls = value;
                RefreshDriverControllerSize();
            }
        }

        public bool KeepMapRatio
        {
            get => _viewbox.Stretch == Stretch.Uniform;
            set => _viewbox.Stretch = value ? Stretch.Uniform : Stretch.Fill;
        }

        public override void UpdateDrivers(SimulatorDataSet dataSet, params DriverInfo[] drivers)
        {
            base.UpdateDrivers(dataSet, drivers);
            UpdateSectorsColor(dataSet);
        }

        private void UpdateSectorsColor(SimulatorDataSet dataSet)
        {
            if (PositionCircleInformationProvider == null || dataSet.PlayerInfo == null)
            {
                return;
            }

            if (dataSet.SessionInfo.SessionType != SessionType.Race)
            {
                UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 1), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 1), _sector1Path);
                UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 2), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 2), _sector2Path);
                UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 3), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 3), _sector3Path);
            }
        }

        private void UpdateSectorColor(bool isSectorGreen, bool isSectorPurple, Shape sectorPath)
        {
            bool shouldBeVisible = isSectorGreen || isSectorPurple;

            if (shouldBeVisible)
            {
                sectorPath.Stroke = isSectorPurple ? PurpleSectorBrush : GreenSectorBrush;
            }

            if ((shouldBeVisible && sectorPath.Opacity < 1) || (!shouldBeVisible && sectorPath.Opacity > 0))
            {
                if (!AnimateDriversPos)
                {
                    sectorPath.Opacity = shouldBeVisible ? 1 : 0;
                }
                else
                {
                    DoubleAnimation newDoubleAnimation = new DoubleAnimation(sectorPath.Opacity, shouldBeVisible ? 1.0 : 0.0, TimeSpan.FromSeconds(0.5));
                    sectorPath.BeginAnimation(Shape.OpacityProperty, newDoubleAnimation);
                }
            }
        }

        protected double DriverControllerSize
        {
            get => (double)GetValue(DriverControllerSizeProperty);
            set => SetValue(DriverControllerSizeProperty, value);
        }

        protected void RefreshDriverControllerSize()
        {
            DriverControllerSize = AutoScaleDriverControls ? _canvasHeight * 0.08 : 25;
            DriverControllerFontSize = DriverControllerSize * 0.75;
        }

        protected override void PostDriverCreation(DriverPositionControl driverPositionControl)
        {
            Binding xBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(DriverPositionControl.WidthProperty, xBinding);

            Binding yBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(DriverPositionControl.HeightProperty, yBinding);

            Binding fontSizeBinding = new Binding(nameof(DriverControllerFontSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(DriverPositionControl.LabelSizeProperty, fontSizeBinding);
        }

        protected override void RemoveDriver(DriverPositionControl driverPositionControl)
        {
            _mainCanvas.Children.Remove(driverPositionControl);
        }

        protected override void AddDriver(DriverPositionControl driverPositionControl)
        {
            _mainCanvas.Children.Add(driverPositionControl);
        }

        protected override double GetDriverControlSize() => DriverControllerSize;

        protected override double GetLabelSize() => DriverControllerFontSize;

        protected override double GetX(DriverInfo driver)
        {
            return driver.WorldPosition.X.InMeters - GetDriverControlSize() / 2;
        }

        protected override double GetY(DriverInfo driver)
        {
            return driver.WorldPosition.Z.InMeters - GetDriverControlSize() / 2;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            _mapSidePanelControl.BeginAnimation(OpacityProperty, showAnimation);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            DoubleAnimation showAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));
            _mapSidePanelControl.BeginAnimation(OpacityProperty, showAnimation);
        }

        private void InitializeMap()
        {
            ClipToBounds = true;
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri(@"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml", UriKind.RelativeOrAbsolute);
            Canvas topCanvas = new Canvas {Width = _canvasWidth, Height = _canvasHeight};

            _mainCanvas = new Canvas();

            Path mainPath = new Path {Stroke = (Brush) resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_fullMapGeometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator))};
            _sector1Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_sector1Geometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator)), Opacity = 0 };
            _sector2Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_sector2Geometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator)), Opacity = 0 };
            _sector3Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_sector3Geometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator)), Opacity = 0 };
            Path finishLinePath = new Path { Stroke = (Brush)resource["LightBlueBrush"], StrokeThickness = 30, Data = Geometry.Parse(_finnishLineGeometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator)) };

            _mainCanvas.Children.Add(mainPath);
            _mainCanvas.Children.Add(_sector1Path);
            _mainCanvas.Children.Add(_sector2Path);
            _mainCanvas.Children.Add(_sector3Path);
            _mainCanvas.Children.Add(finishLinePath);
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = -_leftOffset,
                Y = -_topOffset
            });



            _mainCanvas.RenderTransform = transformGroup;
            topCanvas.Children.Add(_mainCanvas);
            _viewbox = new Viewbox {Stretch = Stretch.Uniform, Child = topCanvas};
            Children.Add(_viewbox);

            _mapSidePanelControl = new MapSidePanelControl {VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left};
            _mapSidePanelControl.Opacity = 0;
            Children.Add(_mapSidePanelControl);
            this.Background = Brushes.Transparent;
        }
    }
}