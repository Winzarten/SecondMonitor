using SecondMonitor.DataModel;
using SecondMonitor.DataModel.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecondMonitor.WindowsControls.wpf
{
    /// <summary>
    /// Interaction logic for PositionCircle.xaml
    /// </summary>
    public partial class PositionCircle : UserControl
    {        
        private const int circleMargin = 15;
        private const int circleDiameter = circleMargin / 2;
        private Ellipse myCircle;        
        private Dictionary<String, Ellipse> driversPoints;
        private Dictionary<String, TextBlock> driverTexts;
        float lapLength;
        public PositionCircle()
        {            
            InitializeComponent();
            CreateCircle();
            driversPoints = new Dictionary<string, Ellipse>();
            driverTexts = new Dictionary<string, TextBlock>();
        }
        

        private void CreateCircle()
        {
            double size;
            if (mainControl.Width < mainControl.Height)
                size = mainControl.Width / 2;
            else
                size = mainControl.Height / 2;
            myCircle = new Ellipse();
            myCircle.Fill = System.Windows.Media.Brushes.Black;
            myCircle.Width = 300;
            myCircle.Height = 300;
            myCircle.StrokeThickness = 3;
            myCircle.Stroke = Brushes.Wheat;
            Canvas.SetLeft(myCircle, circleMargin);
            Canvas.SetTop(myCircle, circleMargin);
            Canvas.SetZIndex(myCircle, 0);
            canvas.Children.Add(myCircle);
        }
        

        public void SetSessionInfo(SimulatorDataSet set)
        {
            lapLength = set.SessionInfo.LayoutLength;
            UnregisterDrivers();
            AddDrivers(set.DriversInfo);
        }

        private void UnregisterDrivers()
        {
            if (driversPoints == null)
                return;
            foreach(var driver in driversPoints.Values)
            {
                canvas.Children.Remove(driver);
            }
            foreach (var driver in driverTexts.Values)
            {
                canvas.Children.Remove(driver);
            }
        }

        public void AddDrivers(DriverInfo[] drivers)
        {
            driversPoints.Clear();
            driverTexts.Clear();
            foreach (var driver in drivers)
            {
                if (driversPoints.ContainsKey(driver.DriverName))
                    continue;
                AddDriver(driver);
            }
        }

        public void AddDriver(DriverInfo driver)
        {
            Ellipse driverEllips = CreateDriverEllipse(driver);
            TextBlock driverTextBlock = CreateDriverText(driver);
            driversPoints[driver.DriverName] = driverEllips;
            driverTexts[driver.DriverName] = driverTextBlock;
        }

        public void RemoveDriver(DriverInfo driver)
        {
            if (driversPoints.ContainsKey(driver.DriverName))
            {
                canvas.Children.Remove(driversPoints[driver.DriverName]);
                driversPoints.Remove(driver.DriverName);
            }
            if (driverTexts.ContainsKey(driver.DriverName))
            {
                canvas.Children.Remove(driverTexts[driver.DriverName]);
                driverTexts.Remove(driver.DriverName);
            }
        }

        private Ellipse CreateDriverEllipse(DriverInfo driver)
        {
            Ellipse driverEllips = new Ellipse();
            if (driver.IsPlayer)
                driverEllips.Fill = Brushes.Red;
            else
                driverEllips.Fill = Brushes.Green;
            driverEllips.Width = circleDiameter * 2;
            driverEllips.Height = circleDiameter *2;
            canvas.Children.Add(driverEllips);
            double x = GetX(driver, canvas.ActualWidth - 10) - circleDiameter;
            double y = GetY(driver, canvas.ActualHeight -10) - circleDiameter;
            Canvas.SetLeft(driverEllips, x);
            Canvas.SetTop(driverEllips, y);
            return driverEllips;
        }

        private TextBlock CreateDriverText(DriverInfo driver)
        {
            TextBlock driverTextBlock = new TextBlock();
            if (driver.IsPlayer)
                driverTextBlock.Foreground = Brushes.Red;
            else
                driverTextBlock.Foreground = Brushes.Green;
            driverTextBlock.Width = 30;
            driverTextBlock.Height = 30;
            driverTextBlock.FontSize = 18;
            driverTextBlock.FontWeight = FontWeights.Bold;
            driverTextBlock.Text = driver.Position.ToString();
            canvas.Children.Add(driverTextBlock);
            double x = GetX(driver, canvas.ActualWidth-15) - circleDiameter;
            double y = GetY(driver, canvas.ActualHeight-15) - circleDiameter;
            Canvas.SetLeft(driverTextBlock, x);
            Canvas.SetTop(driverTextBlock, y);
            return driverTextBlock;
        }

        public void RefreshSession(SimulatorDataSet set)
        {
            foreach(var driver in set.DriversInfo)
            {
                Ellipse driverEllipse; 
                if (!driversPoints.TryGetValue(driver.DriverName, out driverEllipse))
                    continue;
                int margin = circleMargin;
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
                    driverEllipse.Fill = Brushes.Blue;
                else if (driver.IsLapingPlayer)
                    driverEllipse.Fill = Brushes.Red;
                else
                    driverEllipse.Fill = Brushes.Green;
                double x = GetX(driver, canvas.ActualWidth- margin * 2) - circleDiameter;
                double y = GetY(driver, canvas.ActualHeight- margin * 2) - circleDiameter;
                Canvas.SetLeft(driverEllipse, x);
                Canvas.SetTop(driverEllipse, y);

                var text = driverTexts[driver.DriverName];

                if (driver.IsPlayer)
                {
                    text.Foreground = Brushes.Gray;
                }
                else if (driver.InPits)
                {
                    text.Foreground = Brushes.Olive;
                }
                else if (driver.IsBeingLappedByPlayer)
                    text.Foreground = Brushes.Blue;
                else if (driver.IsLapingPlayer)
                    text.Foreground = Brushes.Red;
                else
                    text.Foreground = Brushes.Green;
                text.Text = driver.Position.ToString();
                x = GetX(driver, canvas.ActualWidth- margin * 2 - 40) - 10;
                y = GetY(driver, canvas.ActualHeight- margin * 2 - 40) - 10;
                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, y);
            }
        }

        private double GetX(DriverInfo driver, double ellipseWidth)
        {
            double degrees = (driver.LapDistance / lapLength) * 2*Math.PI - Math.PI / 2;
            double degreesD = (driver.LapDistance / lapLength) * 360;
            double x = canvas.ActualWidth / 2 + (ellipseWidth /2) * Math.Cos(degrees);
            return x;
        }

        private double GetY(DriverInfo driver, double ellipseHeight)
        {
            double degrees = (driver.LapDistance / lapLength) * 2 * Math.PI - Math.PI/2;
            double degreesD = (driver.LapDistance / lapLength) * 360;
            double y = canvas.ActualHeight / 2 + (ellipseHeight / 2) * Math.Sin(degrees);
            return y;
        }


        private void ResazeCircle()
        {
            if (myCircle == null)
                return;
            double size;
            if (canvas.ActualWidth < canvas.ActualHeight)
                size = canvas.ActualWidth;
            else
                size = canvas.ActualHeight;
            myCircle.Width = canvas.ActualWidth - circleMargin * 2;
            myCircle.Height = canvas.ActualHeight - circleMargin * 2;
        }

        private void PositionFinishLine()
        {
            Canvas.SetLeft(finnishLine, canvas.ActualWidth / 2);
            Canvas.SetZIndex(finnishLine, 3);
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResazeCircle();
            PositionFinishLine();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
