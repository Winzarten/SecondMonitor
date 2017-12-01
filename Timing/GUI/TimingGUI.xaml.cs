using System.Windows;
using System.Windows.Controls;

namespace SecondMonitor.Timing.GUI
{
    /// <summary>
    /// Interaction logic for TimingGUI.xaml
    /// </summary>
    public partial class TimingGui : Window
    {        
        public enum TimingModeOptions { Automatic, Absolute, Relative};
        public TimingGui()
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

        private void dtTimig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtTimig.SelectedItem != null)
                dtTimig.SelectedItem = null;
        }
    }
}
