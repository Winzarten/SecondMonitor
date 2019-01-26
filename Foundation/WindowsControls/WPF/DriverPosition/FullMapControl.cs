namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Collections.Generic;
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
    using NLog;

    public class FullMapControl : AbstractSituationOverviewControl
    {

        private readonly ITrackMap _trackMap;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private MapSidePanelControl _mapSidePanelControl;
        private Canvas _mainCanvas;
        private bool _autoScaleDriverControls;
        private Viewbox _viewbox;
        private Path _sector1Path;
        private Path _sector2Path;
        private Path _sector3Path;
        private bool _enableSidePanel;


        public static readonly DependencyProperty DriverControllerSizeProperty = DependencyProperty.Register("DriverControllerSize", typeof(double), typeof(FullMapControl));
        public static readonly DependencyProperty DriverControllerFontSizeProperty = DependencyProperty.Register("DriverControllerFontSize", typeof(double), typeof(FullMapControl));

        public static readonly DependencyProperty YellowSectorBrushProperty = DependencyProperty.Register("YellowSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));
        public static readonly DependencyProperty GreenSectorBrushProperty = DependencyProperty.Register("GreenSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));
        public static readonly DependencyProperty PurpleSectorBrushProperty = DependencyProperty.Register("PurpleSectorBrush", typeof(SolidColorBrush), typeof(FullMapControl));

        public FullMapControl(ITrackMap trackMap)
        {
            _trackMap = trackMap;
            RefreshDriverControllerSize();
            _enableSidePanel = true;
            InitializeMap();
        }

        public bool EnableSidePanel
        {
            get => _enableSidePanel;
            set
            {
                _enableSidePanel = value;
                _mapSidePanelControl.Visibility = _enableSidePanel ? Visibility.Visible : Visibility.Collapsed;
            }
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

        public override void UpdateDrivers(SimulatorDataSet dataSet, params IDriverInfo[] drivers)
        {
            base.UpdateDrivers(dataSet, drivers);
            UpdateSectorsColor(dataSet);
        }

        public void AddCustomPath(Path path)
        {
            _mainCanvas.Children.Add(path);
        }

        public void RemoveCustomPath(Path path)
        {
            _mainCanvas.Children.Remove(path);
        }

        private void UpdateSectorsColor(SimulatorDataSet dataSet)
        {
            if (PositionCircleInformationProvider == null || dataSet?.PlayerInfo == null)
            {
                return;
            }

            bool showSector1 = false;
            bool showSector2 = false;
            bool showSector3 = false;

            foreach (FlagKind flag in dataSet.SessionInfo.ActiveFlags)
            {
                if (flag == FlagKind.FullCourseYellow || flag == FlagKind.VirtualSafetyCar || flag == FlagKind.FullCourseYellow)
                {
                    showSector1 = true;
                    showSector2 = true;
                    showSector3 = true;
                    break;
                }

                switch (flag)
                {
                    case FlagKind.YellowSector1:
                        showSector1 = true;
                        break;
                    case FlagKind.YellowSector2:
                        showSector2 = true;
                        break;
                    case FlagKind.YellowSector3:
                        showSector3 = true;
                        break;
                }
            }

            UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 1), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 1), showSector1, _sector1Path);
            UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 2), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 2), showSector2, _sector2Path);
            UpdateSectorColor(PositionCircleInformationProvider.IsDriverLastSectorGreen(dataSet.PlayerInfo, 3), PositionCircleInformationProvider.IsDriverLastSectorPurple(dataSet.PlayerInfo, 3), showSector3, _sector3Path);
        }

        private void UpdateSectorColor(bool isSectorGreen, bool isSectorPurple, bool isSectorYellow, Shape sectorPath)
        {
            bool shouldBeVisible = isSectorGreen || isSectorPurple || isSectorYellow;
            SolidColorBrush brushToUse = null;

            if (shouldBeVisible && isSectorGreen)
            {
                brushToUse = GreenSectorBrush;
            }

            if (shouldBeVisible && isSectorPurple)
            {
                brushToUse = PurpleSectorBrush;
            }

            if (shouldBeVisible && isSectorYellow)
            {
                brushToUse = YellowSectorBrush;
            }

            if (brushToUse != null && brushToUse != sectorPath.Stroke)
            {
                sectorPath.Stroke = brushToUse;
            }

            if ((shouldBeVisible && sectorPath.Opacity == 0) || (!shouldBeVisible && sectorPath.Opacity == 1))
            {
                if (!AnimateDriversPos)
                {
                    sectorPath.Opacity = shouldBeVisible ? 1 : 0;
                }
                else
                {
                    DoubleAnimation newDoubleAnimation = new DoubleAnimation(sectorPath.Opacity, shouldBeVisible ? 1.0 : 0.0, TimeSpan.FromSeconds(0.5));
                    sectorPath.BeginAnimation(OpacityProperty, newDoubleAnimation);
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
            DriverControllerSize = AutoScaleDriverControls ? _trackMap.TrackGeometry.Height * 0.08 : 15;
            DriverControllerFontSize = DriverControllerSize * 0.75;
        }

        protected override void PostDriverCreation(DriverPositionControl driverPositionControl)
        {
            Binding xBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(WidthProperty, xBinding);

            Binding yBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(HeightProperty, yBinding);
        }

        protected override void RemoveDriver(DriverPositionControl driverPositionControl)
        {
            if (_mainCanvas.Children.Contains(driverPositionControl))
            {
                _mainCanvas.Children.Remove(driverPositionControl);
            }
        }

        protected override void AddDriver(DriverPositionControl driverPositionControl)
        {
            _mainCanvas.Children.Add(driverPositionControl);
        }

        protected override double GetDriverControlSize() => DriverControllerSize;

        protected override double GetLabelSize() => DriverControllerFontSize;

        protected override double GetX(IDriverInfo driver)
        {
            double xCoord = _trackMap.TrackGeometry.IsSwappedAxis ? driver.WorldPosition.Z.InMeters * _trackMap.TrackGeometry.YCoef : driver.WorldPosition.X.InMeters * _trackMap.TrackGeometry.XCoef;
            return xCoord - GetDriverControlSize() / 2;
        }

        protected override double GetY(IDriverInfo driver)
        {
            double yCoord = _trackMap.TrackGeometry.IsSwappedAxis ? driver.WorldPosition.X.InMeters * _trackMap.TrackGeometry.XCoef : driver.WorldPosition.Z.InMeters * _trackMap.TrackGeometry.YCoef;
            return yCoord - GetDriverControlSize() / 2;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!EnableSidePanel)
            {
                return;
            }
            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            _mapSidePanelControl.BeginAnimation(OpacityProperty, showAnimation);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!EnableSidePanel)
            {
                return;
            }
            DoubleAnimation showAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));
            _mapSidePanelControl.BeginAnimation(OpacityProperty, showAnimation);
        }

        private void InitializeMap()
        {
            Logger.Info("Creating Map");
            Logger.Info($"Geometry: {_trackMap.TrackGeometry.FullMapGeometry}");
            Logger.Info($"Decimal Separator: {CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}");
            ClipToBounds = true;
            ResourceDictionary resource = new ResourceDictionary {Source = new Uri(@"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml", UriKind.RelativeOrAbsolute)};
            Canvas topCanvas = new Canvas {Width = _trackMap.TrackGeometry.Width, Height = _trackMap.TrackGeometry.Height};

            _mainCanvas = new Canvas();

            Path mainPath = new Path {Stroke = (Brush) resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_trackMap.TrackGeometry.FullMapGeometry)};
            _sector1Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_trackMap.TrackGeometry.Sector1Geometry), Opacity = 0 };
            _sector2Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_trackMap.TrackGeometry.Sector2Geometry), Opacity = 0 };
            _sector3Path = new Path { Stroke = (Brush)resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_trackMap.TrackGeometry.Sector3Geometry), Opacity = 0 };
            Path finishLinePath = new Path { Stroke = (Brush)resource["LightBlueBrush"], StrokeThickness = 30, Data = Geometry.Parse(_trackMap.TrackGeometry.StartLineGeometry)};

            _mainCanvas.Children.Add(mainPath);
            _mainCanvas.Children.Add(_sector1Path);
            _mainCanvas.Children.Add(_sector2Path);
            _mainCanvas.Children.Add(_sector3Path);
            _mainCanvas.Children.Add(finishLinePath);
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = -_trackMap.TrackGeometry.LeftOffset,
                Y = -_trackMap.TrackGeometry.TopOffset
            });



            _mainCanvas.RenderTransform = transformGroup;
            topCanvas.Children.Add(_mainCanvas);
            _viewbox = new Viewbox {Stretch = Stretch.Uniform, Child = topCanvas};
            Children.Add(_viewbox);

            _mapSidePanelControl = new MapSidePanelControl {VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left};
            _mapSidePanelControl.Opacity = 0;
            Children.Add(_mapSidePanelControl);
            Background = Brushes.Transparent;
        }
    }
}