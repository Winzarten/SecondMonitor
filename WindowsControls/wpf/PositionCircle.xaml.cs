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
        public PositionCircle()
        {            
            InitializeComponent();
            double size;
            if (mainControl.Width < mainControl.Height)
                size = mainControl.Width / 2;
            else
                size = mainControl.Height / 2;
            myCircle = new Ellipse();
            myCircle.Fill = System.Windows.Media.Brushes.Yellow;
            myCircle.Width = size;
            myCircle.Height = size;
            myCircle.StrokeThickness = 2;
            myCircle.Stroke = Brushes.Black;            
            canvas.Children.Add(myCircle);
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (myCircle == null)
                return;
            double size;
            if (mainControl.Width < mainControl.Height)
                size = mainControl.Width / 2;
            else
                size = mainControl.Height / 2;
            myCircle.Width = size;
            myCircle.Height = size;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Children.Remove(myCircle);
               myCircle = new Ellipse();
            myCircle.Fill = System.Windows.Media.Brushes.Yellow;
            if (myCircle == null)
                return;
            double size;
            if (mainControl.ActualHeight < mainControl.ActualHeight)
                size = mainControl.ActualWidth / 2;
            else
                size = mainControl.ActualHeight / 2;
            myCircle.Width = size;
            myCircle.Height = size;
            myCircle.Stroke = Brushes.Black;            
            canvas.Children.Add(myCircle);
        }
    }
}
