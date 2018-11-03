namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
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

        private Canvas _mainCanvas;

        public FullMapControl(ITrackMap trackMap)
        {
            _topOffset = trackMap.TrackGeometry.TopOffset;
            _leftOffset = trackMap.TrackGeometry.LeftOffset;
            _canvasWidth = trackMap.TrackGeometry.Width;
            _canvasHeight = trackMap.TrackGeometry.Height;
            _fullMapGeometry = trackMap.TrackGeometry.FullMapGeometry;
            _finnishLineGeometry = trackMap.TrackGeometry.StartLineGeometry;
            InitializeMap();
        }

        protected override void RemoveDriver(DriverPositionControl driverPositionControl)
        {
            _mainCanvas.Children.Remove(driverPositionControl);
        }

        protected override void AddDriver(DriverPositionControl driverPositionControl)
        {
            _mainCanvas.Children.Add(driverPositionControl);
        }

        protected override double GetDriverControlSize()
        {
            return _canvasHeight * 0.08;
        }

        protected override double GetX(DriverInfo driver)
        {
            return driver.WorldPosition.X.InMeters - GetDriverControlSize() / 2;
        }

        protected override double GetY(DriverInfo driver)
        {
            return driver.WorldPosition.Z.InMeters - GetDriverControlSize() / 2;
        }

        private void InitializeMap()
        {
            ClipToBounds = true;
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri(@"pack://application:,,,/WindowsControls;component/WPF/CommonResources.xaml", UriKind.RelativeOrAbsolute);
            Canvas topCanvas = new Canvas {Width = _canvasWidth, Height = _canvasHeight};

            _mainCanvas = new Canvas();

            Path mainPath = new Path {Stroke = (Brush) resource["DarkGrey01Brush"], StrokeThickness = 15, Data = Geometry.Parse(_fullMapGeometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator))};
            Path finishLinePath = new Path { Stroke = (Brush)resource["LightBlueBrush"], StrokeThickness = 30, Data = Geometry.Parse(_finnishLineGeometry.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator)) };

            _mainCanvas.Children.Add(mainPath);
            _mainCanvas.Children.Add(finishLinePath);
            _mainCanvas.RenderTransform = new TranslateTransform()
            {
                X = -_leftOffset,
                Y = -_topOffset
            };

            topCanvas.Children.Add(_mainCanvas);

            Viewbox viewbox = new Viewbox {Stretch = Stretch.Uniform, Child = topCanvas};
            Children.Add(viewbox);
        }
    }
}