namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;

    public class PositionCircleControl : Grid, ISituationOverviewControl
    {
        private static readonly DependencyProperty PlayerForegroundBrushProperty = DependencyProperty.Register("PlayerForegroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty PlayerBackgroundBrushProperty = DependencyProperty.Register("PlayerBackgroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty DriverForegroundBrushProperty = DependencyProperty.Register("DriverForegroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty DriverBackgroundBrushProperty = DependencyProperty.Register("DriverBackgroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty LappedDriverForegroundBrushProperty = DependencyProperty.Register("LappedDriverForegroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty LappedDriverBackgroundBrushProperty = DependencyProperty.Register("LappedDriverBackgroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty LappingDriverForegroundBrushProperty = DependencyProperty.Register("LappingDriverForegroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty LappingDriverBackgroundBrushProperty = DependencyProperty.Register("LappingDriverBackgroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty DriverPitsForegroundBrushProperty = DependencyProperty.Register("DriverPitsForegroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));
        private static readonly DependencyProperty DriverPitsBackgroundBrushProperty = DependencyProperty.Register("DriverPitsBackgroundBrush", typeof(SolidColorBrush), typeof(PositionCircleControl));

        private readonly Dictionary<string, DriverPositionControl> _drivers;

        private bool _animateDriversPosition;

        public PositionCircleControl()
        {
            _drivers = new Dictionary<string, DriverPositionControl>();
            SetUpControl();
        }

        public SolidColorBrush PlayerForegroundBrush
        {
            get => (SolidColorBrush)GetValue(PlayerForegroundBrushProperty);
            set => SetValue(PlayerForegroundBrushProperty, value);
        }

        public SolidColorBrush PlayerBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(PlayerBackgroundBrushProperty);
            set => SetValue(PlayerBackgroundBrushProperty, value);
        }

        public SolidColorBrush DriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverForegroundBrushProperty);
            set => SetValue(DriverForegroundBrushProperty, value);
        }

        public SolidColorBrush DriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverBackgroundBrushProperty);
            set => SetValue(DriverBackgroundBrushProperty, value);
        }

        public SolidColorBrush DriverPitsForegroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverPitsForegroundBrushProperty);
            set => SetValue(DriverPitsForegroundBrushProperty, value);
        }

        public SolidColorBrush DriverPitsBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(DriverPitsBackgroundBrushProperty);
            set => SetValue(DriverPitsBackgroundBrushProperty, value);
        }

        public SolidColorBrush LappedDriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(LappedDriverForegroundBrushProperty);
            set => SetValue(LappedDriverForegroundBrushProperty, value);
        }

        public SolidColorBrush LappedDriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(LappedDriverBackgroundBrushProperty);
            set => SetValue(LappedDriverBackgroundBrushProperty, value);
        }

        public SolidColorBrush LappingDriverForegroundBrush
        {
            get => (SolidColorBrush)GetValue(LappingDriverForegroundBrushProperty);
            set => SetValue(LappingDriverForegroundBrushProperty, value);
        }

        public SolidColorBrush LappingDriverBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(LappingDriverBackgroundBrushProperty);
            set => SetValue(LappingDriverBackgroundBrushProperty, value);
        }

        public bool AnimateDriversPos
        {
            get => _animateDriversPosition;
            set
            {
                _animateDriversPosition = value;
                _drivers.Values.ForEach(x => x.Animate = value);
            }
        }


        public IPositionCircleInformationProvider PositionCircleInformationProvider
        {
            get;
            set;
        }

        private SimulatorDataSet LastDataSet { get; set; }

        public void AddDrivers(params DriverInfo[] drivers)
        {
            foreach (DriverInfo driver in drivers)
            {
                AddDriver(driver);
            }
        }

        public void RemoveDrivers(params DriverInfo[] drivers)
        {
            foreach (DriverInfo driver in drivers)
            {
                RemoveDriver(driver.DriverName);
            }
        }

        public void UpdateDrivers(SimulatorDataSet dataSet, params DriverInfo[] drivers)
        {
            LastDataSet = dataSet;
            foreach (DriverInfo driver in drivers)
            {
                if (_drivers.ContainsKey(driver.DriverName))
                {
                    UpdateDriver(driver, _drivers[driver.DriverName]);
                }
                else
                {
                    AddDriver(driver);
                }
            }
        }

        public void RemoveAllDrivers()
        {
            foreach (DriverPositionControl control in _drivers.Values)
            {
                Children.Remove(control);
            }

            _drivers.Clear();
        }


        private void AddDriver(DriverInfo driverInfo)
        {
            DriverPositionControl newDriverControl =
                new DriverPositionControl
                    {
                        Width = 25,
                        Height = 25,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Animate = AnimateDriversPos
                    };
            UpdateDriver(driverInfo, newDriverControl);
            Children.Add(newDriverControl);
            _drivers.Add(driverInfo.DriverName, newDriverControl);
        }

        private void RemoveDriver(string driverName)
        {
            if (!_drivers.ContainsKey(driverName))
            {
                return;
            }

            DriverPositionControl driverToRemove = _drivers[driverName];
            Children.Remove(driverToRemove);
            _drivers.Remove(driverName);
        }

        private void UpdateDriver(DriverInfo driverInfo, DriverPositionControl driverPositionControl)
        {
            if (LastDataSet == null)
            {
                return;
            }

            driverPositionControl.Position = driverInfo.Position;
            driverPositionControl.X = GetX(driverInfo);
            driverPositionControl.Y = GetY(driverInfo);
            UpdateColors(driverInfo, driverPositionControl);
        }

        private void UpdateColors(DriverInfo driverInfo, DriverPositionControl driverPositionControl)
        {
            if (driverInfo.IsPlayer)
            {
                driverPositionControl.CircleBrush = PlayerBackgroundBrush;
                driverPositionControl.TextBrush = PlayerForegroundBrush;
                SetZIndex(driverPositionControl, 100);
                return;
            }

            if (driverInfo.InPits)
            {
                driverPositionControl.CircleBrush = DriverPitsBackgroundBrush;
                driverPositionControl.TextBrush = DriverPitsForegroundBrush;
                return;
            }

            if (IsLapped(driverInfo))
            {
                driverPositionControl.CircleBrush = LappedDriverBackgroundBrush;
                driverPositionControl.TextBrush = LappedDriverForegroundBrush;
                return;
            }

            if (driverInfo.IsLappingPlayer)
            {
                driverPositionControl.CircleBrush = LappingDriverBackgroundBrush;
                driverPositionControl.TextBrush = LappingDriverForegroundBrush;
                return;
            }

            driverPositionControl.CircleBrush = DriverBackgroundBrush;
            driverPositionControl.TextBrush = DriverForegroundBrush;
        }

        private bool IsLapped(DriverInfo driver)
        {
            if (LastDataSet.SessionInfo.SessionType == SessionType.Race || PositionCircleInformationProvider == null)
            {
                return driver.IsBeingLappedByPlayer;
            }

            return !PositionCircleInformationProvider.IsDriverOnValidLap(driver);
        }

        private double GetX(DriverInfo driver)
        {
            double lapLength = LastDataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            double degrees = (driver.LapDistance / lapLength) * 2 * Math.PI - Math.PI / 2;
            double x = (ActualWidth / 2) * Math.Cos(degrees);
            return double.IsNaN(x) ? 0 : x;
        }

        private double GetY(DriverInfo driver)
        {
            double lapLength = LastDataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            double degrees = ((driver.LapDistance / lapLength) * 2 * Math.PI) - (Math.PI / 2);
            double y = (ActualHeight / 2) * Math.Sin(degrees);

            if (driver.InPits)
            {
                y += 30;
            }

            return double.IsNaN(y) ? 0 : y;
        }


        private void SetUpControl()
        {
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri(@"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml", UriKind.RelativeOrAbsolute);
            SolidColorBrush ellipseStroke = (SolidColorBrush)resource["DarkGrey05Brush"];
            Grid grid = new Grid
                            {
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

            Line line = new Line { Y1 = -10, Y2 = 10, Stroke = (Brush)resource["Green01Brush"], StrokeThickness = 4};
            grid.Children.Add(line);
            Children.Add(grid);

            Ellipse ellipse = new Ellipse()
                                  {
                                      VerticalAlignment = VerticalAlignment.Stretch,
                                      HorizontalAlignment = HorizontalAlignment.Stretch,
                                      Stroke = ellipseStroke,
                                      StrokeThickness = 3
                                  };
            Children.Add(ellipse);
        }
    }
}