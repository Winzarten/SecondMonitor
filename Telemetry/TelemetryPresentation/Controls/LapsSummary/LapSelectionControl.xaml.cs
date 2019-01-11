using System.Windows;
using System.Windows.Controls;

namespace SecondMonitor.TelemetryPresentation.Controls.LapsSummary
{
    using System;
    using OpenWindow;
    using Telemetry.TelemetryApplication.ViewModels.LapPicker;
    using Telemetry.TelemetryApplication.ViewModels.OpenWindow;

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
            IOpenWindowViewModel openWindowViewModel = ((ILapSelectionViewModel)DataContext).OpenWindowViewModel;

            if (_openWindow == null )
            {
                openWindowViewModel.IsOpenWindowVisible = true;
                _openWindow = new OpenWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen, Owner = Window.GetWindow(this), DataContext = openWindowViewModel };
                _openWindow.Closed+= OpenWindowOnClosed;
                _openWindow.Show();
                return;
            }

            openWindowViewModel.IsOpenWindowVisible = true;
            _openWindow.Focus();
        }

        private void OpenWindowOnClosed(object sender, EventArgs e)
        {
            _openWindow.DataContext = null;
            _openWindow.Closed -= OpenWindowOnClosed;
            _openWindow = null;
        }
    }
}
