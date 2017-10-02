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

namespace SecondMonitor.Timing.GUI
{
    /// <summary>
    /// Interaction logic for TimingGUI.xaml
    /// </summary>
    public partial class TimingGUI : Window
    {        
        public enum TimingModeOptions { Automatic, Absolute, Relative};
        public TimingGUI()
        {
            InitializeComponent();
        }

        public TimingModeOptions TimingMode
        {
            get
            {
                if ((bool)rbtAutomatic.IsChecked)
                    return TimingModeOptions.Automatic;
                if ((bool)rbtRelative.IsChecked)
                    return TimingModeOptions.Relative;
                if ((bool)rbtAbsolute.IsChecked)
                    return TimingModeOptions.Absolute;
                return TimingModeOptions.Automatic;
            }
        }
    }
}
