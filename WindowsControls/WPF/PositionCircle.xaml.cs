namespace SecondMonitor.WindowsControls.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Drivers;

    /// <summary>
    /// Interaction logic for PositionCircle.xaml
    /// </summary>
    public partial class PositionCircle : UserControl
    {        
        private const int CircleMargin = 15;
        private const int CircleDiameter = CircleMargin / 2;
        private Ellipse _myCircle;        
        private Dictionary<string, Ellipse> _driversPoints;
        private Dictionary<string, TextBlock> _driverTexts;
        float _lapLength;
        public PositionCircle()
        {            
            InitializeComponent();
            CreateCircle();
            _driversPoints = new Dictionary<string, Ellipse>();
            _driverTexts = new Dictionary<string, TextBlock>();
        }
        

        private void CreateCircle()
        {
            _myCircle = new Ellipse();
            _myCircle.Fill = Brushes.Black;
            _myCircle.Width = 300;
            _myCircle.Height = 300;
            _myCircle.StrokeThickness = 3;
            _myCircle.Stroke = Brushes.Wheat;
            Canvas.SetLeft(_myCircle, CircleMargin);
            Canvas.SetTop(_myCircle, CircleMargin);
            Panel.SetZIndex(_myCircle, 0);
            canvas.Children.Add(_myCircle);
        }
        

        public void SetSessionInfo(SimulatorDataSet set)
        {
            _lapLength = set.SessionInfo.TrackInfo.LayoutLength;
            UnregisterDrivers();
            AddDrivers(set.DriversInfo);
        }

        private void UnregisterDrivers()
        {
            if (_driversPoints == null)
            {
                return;
            }
            foreach(var driver in _driversPoints.Values)
            {
                canvas.Children.Remove(driver);
            }

            foreach (var driver in _driverTexts.Values)
            {
                canvas.Children.Remove(driver);
            }
        }

        public void AddDrivers(DriverInfo[] drivers)
        {
            _driversPoints.Clear();
            _driverTexts.Clear();
            foreach (var driver in drivers)
            {
                if (_driversPoints.ContainsKey(driver.DriverName))
                {
                    continue;
                }
                AddDriver(driver);
            }
        }

        public void AddDriver(DriverInfo driver)
        {
            Ellipse driverEllips = CreateDriverEllipse(driver);
            TextBlock driverTextBlock = CreateDriverText(driver);
            _driversPoints[driver.DriverName] = driverEllips;
            _driverTexts[driver.DriverName] = driverTextBlock;
        }

        public void RemoveDriver(DriverInfo driver)
        {
            if (_driversPoints.ContainsKey(driver.DriverName))
            {
                canvas.Children.Remove(_driversPoints[driver.DriverName]);
                _driversPoints.Remove(driver.DriverName);
            }

            if (_driverTexts.ContainsKey(driver.DriverName))
            {
                canvas.Children.Remove(_driverTexts[driver.DriverName]);
                _driverTexts.Remove(driver.DriverName);
            }
        }

        private Ellipse CreateDriverEllipse(DriverInfo driver)
        {
            Ellipse driverEllips = new Ellipse();
            driverEllips.Fill = driver.IsPlayer ? Brushes.Red : Brushes.Green;
            driverEllips.Width = CircleDiameter * 2;
            driverEllips.Height = CircleDiameter * 2;
            canvas.Children.Add(driverEllips);
            double x = GetX(driver, canvas.ActualWidth - 10) - CircleDiameter;
            double y = GetY(driver, canvas.ActualHeight - 10) - CircleDiameter;
            Canvas.SetLeft(driverEllips, x);
            Canvas.SetTop(driverEllips, y);
            return driverEllips;
        }

        private TextBlock CreateDriverText(DriverInfo driver)
        {
            TextBlock driverTextBlock = new TextBlock();
            driverTextBlock.Foreground = driver.IsPlayer ? Brushes.Red : Brushes.Green;
            driverTextBlock.Width = 30;
            driverTextBlock.Height = 30;
            driverTextBlock.FontSize = 18;
            driverTextBlock.FontWeight = FontWeights.Bold;
            driverTextBlock.Text = driver.Position.ToString();
            canvas.Children.Add(driverTextBlock);
            double x = GetX(driver, canvas.ActualWidth - 15) - CircleDiameter;
            double y = GetY(driver, canvas.ActualHeight - 15) - CircleDiameter;
            Canvas.SetLeft(driverTextBlock, x);
            Canvas.SetTop(driverTextBlock, y);
            return driverTextBlock;
        }

        public void RefreshSession(SimulatorDataSet set)
        {
            foreach(var driver in set.DriversInfo)
            {
                Ellipse driverEllipse;
                if (!_driversPoints.TryGetValue(driver.DriverName, out driverEllipse))
                {
                    continue;
                }
                int margin = CircleMargin;
                if (driver.InPits)
                {
                    margin = margin + 10;
                }

                if (driver.IsPlayer)
                {
                    driverEllipse.Fill = Brushes.Gray;
                }
                else if (driver.InPits)
                {
                    driverEllipse.Fill = Brushes.Olive;
                }
                else if (driver.IsBeingLappedByPlayer)
                { 
                    driverEllipse.Fill = Brushes.Blue;
                }
                else if (driver.IsLappingPlayer)
                {
                    driverEllipse.Fill = Brushes.Red;
                }
                else
                {
                    driverEllipse.Fill = Brushes.Green;
                }
                double x = GetX(driver, canvas.ActualWidth- margin * 2) - CircleDiameter;
                double y = GetY(driver, canvas.ActualHeight- margin * 2) - CircleDiameter;
                Canvas.SetLeft(driverEllipse, x);
                Canvas.SetTop(driverEllipse, y);

                var text = _driverTexts[driver.DriverName];

                if (driver.IsPlayer)
                {
                    text.Foreground = Brushes.Gray;
                }
                else if (driver.InPits)
                {
                    text.Foreground = Brushes.Olive;
                }
                else if (driver.IsBeingLappedByPlayer)
                {
                    text.Foreground = Brushes.Blue;
                }
                else if (driver.IsLappingPlayer)
                {
                    text.Foreground = Brushes.Red;
                }
                else
                {
                    text.Foreground = Brushes.Green;
                }
                text.Text = driver.Position.ToString();
                x = GetX(driver, canvas.ActualWidth- margin * 2 - 40) - 10;
                y = GetY(driver, canvas.ActualHeight- margin * 2 - 40) - 10;
                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, y);
            }
        }

        private double GetX(DriverInfo driver, double ellipseWidth)
        {
            double degrees = (driver.LapDistance / _lapLength) * 2 * Math.PI - Math.PI / 2;
            double x = canvas.ActualWidth / 2 + (ellipseWidth / 2) * Math.Cos(degrees);
            return x;
        }

        private double GetY(DriverInfo driver, double ellipseHeight)
        {
            double degrees = (driver.LapDistance / _lapLength) * 2 * Math.PI - Math.PI/2;
            double y = canvas.ActualHeight / 2 + (ellipseHeight / 2) * Math.Sin(degrees);
            return y;
        }


        private void ResizeCircle()
        {
            if (_myCircle == null)
            {
                return;
            }
            _myCircle.Width = canvas.ActualWidth - CircleMargin * 2;
            _myCircle.Height = canvas.ActualHeight - CircleMargin * 2;
        }

        private void PositionFinishLine()
        {
            Canvas.SetLeft(finnishLine, canvas.ActualWidth / 2);
            Panel.SetZIndex(finnishLine, 3);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ResizeCircle();
            PositionFinishLine();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
