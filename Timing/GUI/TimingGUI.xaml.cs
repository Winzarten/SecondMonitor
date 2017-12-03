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
                if ((bool)RbtAutomatic.IsChecked)
                    return TimingModeOptions.Automatic;
                if ((bool)RbtRelative.IsChecked)
                    return TimingModeOptions.Relative;
                if ((bool)RbtAbsolute.IsChecked)
                    return TimingModeOptions.Absolute;
                return TimingModeOptions.Automatic;
            }
        }

        private void dtTimig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DtTimig.SelectedItem != null)
                DtTimig.SelectedItem = null;
        }
    }
}
