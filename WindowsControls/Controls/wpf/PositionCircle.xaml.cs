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
        private Ellipse myCircle;
        private Dictionary<String, Ellipse> driversPoints;
        float lapLength;
        public PositionCircle()
        {            
            InitializeComponent();
            CreateCircle();
            driversPoints = new Dictionary<string, Ellipse>();
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
            Canvas.SetLeft(myCircle, 30);
            Canvas.SetTop(myCircle, 30);
            canvas.Children.Add(myCircle);
        }
        

        public void SetSessionInfo(SimulatorDataSet set)
        {
            lapLength = set.SessionInfo.LayoutLength;
            UnregisterDrivers();
            RegisterNewDrivers(set.DriversInfo);
        }

        private void UnregisterDrivers()
        {
            if (driversPoints == null)
                return;
            foreach(var driver in driversPoints.Keys)
            {
                canvas.Children.Remove(driversPoints[driver]);
            }
        }

        private void RegisterNewDrivers(DriverInfo[] drivers)
        {
            driversPoints.Clear();
            foreach (var driver in drivers)
            {
                Ellipse driverEllips = new Ellipse();
                if(driver.IsPlayer)
                    driverEllips.Fill = Brushes.Red;
                else
                    driverEllips.Fill = Brushes.Blue;
                driverEllips.Width = 10;
                driverEllips.Height = 10;
                canvas.Children.Add(driverEllips);
                double x = GetX(driver) -5;
                double y = GetY(driver) -5;
                Canvas.SetLeft(driverEllips, x);
                Canvas.SetTop(driverEllips, y);
                driversPoints[driver.DriverName] = driverEllips;
            }
        }

        public void RefreshSession(SimulatorDataSet set)
        {
            foreach(var driver in set.DriversInfo)
            {
                var driverEllipse = driversPoints[driver.DriverName];
                double x = GetX(driver) - 5;
                double y = GetY(driver) - 5;
                Canvas.SetLeft(driverEllipse, x);
                Canvas.SetTop(driverEllipse, y);
            }
        }

        private double GetX(DriverInfo driver)
        {
            double degrees = (driver.LapDistance / lapLength) * 2*Math.PI - Math.PI / 2;
            double degreesD = (driver.LapDistance / lapLength) * 360;
            double x = canvas.ActualWidth / 2 + (myCircle.ActualWidth/2) * Math.Cos(degrees);
            return x;
        }

        private double GetY(DriverInfo driver)
        {
            double degrees = (driver.LapDistance / lapLength) * 2 * Math.PI - Math.PI/2;
            double degreesD = (driver.LapDistance / lapLength) * 360;
            double y = canvas.ActualHeight / 2 + (myCircle.ActualHeight / 2) * Math.Sin(degrees);
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
            myCircle.Width = canvas.ActualWidth - 60;
            myCircle.Height = canvas.ActualHeight - 60;
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResazeCircle();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
