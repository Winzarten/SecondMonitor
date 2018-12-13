namespace SecondMonitor.WindowsControls.WPF.DriverPosition
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using DataModel.Snapshot.Drivers;

    public class PositionCircleControl : AbstractSituationOverviewControl
    {

        public PositionCircleControl() : base()
        {
            SetUpControl();
        }

        protected override void PostDriverCreation(DriverPositionControl driverPositionControl)
        {
            //Nothing to do
        }

        protected override void RemoveDriver(DriverPositionControl driverPositionControl)
        {
            Children.Remove(driverPositionControl);
        }

        protected override void AddDriver(DriverPositionControl driverPositionControl)
        {
            Children.Add(driverPositionControl);
        }

        protected override double GetDriverControlSize()
        {
            return 25;
        }

        protected override double GetLabelSize()
        {
            return 20;
        }

        protected override double GetX(DriverInfo driver)
        {
            double lapLength = LastDataSet.SessionInfo.TrackInfo.LayoutLength.InMeters;
            double degrees = (driver.LapDistance / lapLength) * 2 * Math.PI - Math.PI / 2;
            double x = (ActualWidth / 2) * Math.Cos(degrees);
            return double.IsNaN(x) ? 0 : x;
        }

        protected override double GetY(DriverInfo driver)
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
            TextBlock informationTextBox = new TextBlock {Style = (Style) resource["StandardText"], VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left};
            Binding textBinding = new Binding(nameof(AdditionalInformation)) {Source = this};
            informationTextBox.SetBinding(TextBlock.TextProperty, textBinding);
            Children.Add(informationTextBox);
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