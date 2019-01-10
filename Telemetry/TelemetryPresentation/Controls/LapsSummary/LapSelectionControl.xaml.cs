using System.Windows;
using System.Windows.Controls;

namespace SecondMonitor.TelemetryPresentation.Controls.LapsSummary
{
    using OpenWindow;
    using Telemetry.TelemetryApplication.ViewModels.LapPicker;

    /// <summary>
    /// Interaction logic for LapSelectionControl.xaml
    /// </summary>
    public partial class LapSelectionControl : UserControl
    {
        private OpenWindow _openWindow;

        public LapSelectionControl()
        {
            InitializeComponent();
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_openWindow == null || !_openWindow.IsVisible)
            {
                _openWindow = new OpenWindow {WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = Window.GetWindow(this), DataContext = ((ILapSelectionViewModel) DataContext).OpenWindowViewModel};
                _openWindow.Show();
                return;
            }

            _openWindow.Focus();
        }
    }
}
