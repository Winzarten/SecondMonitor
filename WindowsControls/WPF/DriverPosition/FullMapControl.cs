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

        private MapSidePanelControl _mapSidePanelControl;
        private Canvas _mainCanvas;
        private bool _autoScaleDriverControls;

        public static readonly DependencyProperty DriverControllerSizeProperty = DependencyProperty.Register("DriverControllerSize", typeof(double), typeof(FullMapControl));

        public FullMapControl(ITrackMap trackMap)
        {
            _topOffset = trackMap.TrackGeometry.TopOffset;
            _leftOffset = trackMap.TrackGeometry.LeftOffset;
            _canvasWidth = trackMap.TrackGeometry.Width;
            _canvasHeight = trackMap.TrackGeometry.Height;
            _fullMapGeometry = trackMap.TrackGeometry.FullMapGeometry;
            _finnishLineGeometry = trackMap.TrackGeometry.StartLineGeometry;
            RefreshDriverControllerSize();
            InitializeMap();
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

        protected double DriverControllerSize
        {
            get => (double)GetValue(DriverControllerSizeProperty);
            set => SetValue(DriverControllerSizeProperty, value);
        }

        protected void RefreshDriverControllerSize()
        {
            DriverControllerSize = AutoScaleDriverControls ? _canvasHeight * 0.08 : 25;
        }

        protected override void PostDriverCreation(DriverPositionControl driverPositionControl)
        {
            Binding xBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(DriverPositionControl.XProperty, xBinding);

            Binding yBinding = new Binding(nameof(DriverControllerSize))
            {
                Source = this,
            };
            driverPositionControl.SetBinding(DriverPositionControl.YProperty, yBinding);
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

        protected override double GetLabelSize()
        {
            return GetDriverControlSize() * 0.75;
        }

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

            _mapSidePanelControl = new MapSidePanelControl {VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left};
            _mapSidePanelControl.Opacity = 0;
            Children.Add(_mapSidePanelControl);
            this.Background = Brushes.Transparent;
        }
    }
}