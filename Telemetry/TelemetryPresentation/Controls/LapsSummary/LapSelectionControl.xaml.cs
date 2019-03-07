using System.Windows;
using System.Windows.Controls;

namespace SecondMonitor.TelemetryPresentation.Controls.LapsSummary
{
    using System;
    using OpenWindow;
    using SettingsWindow;
    using Telemetry.TelemetryApplication.ViewModels.LapPicker;
    using Telemetry.TelemetryApplication.ViewModels.OpenWindow;
    using Telemetry.TelemetryApplication.ViewModels.SettingsWindow;

    /// <summary>
    /// Interaction logic for LapSelectionControl.xaml
    /// </summary>
    public partial class LapSelectionControl : UserControl
    {
        private OpenWindow _openWindow;
        private OpenWindow _addWindow;
        private SettingsWindow _settingsWindow;

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
                openWindowViewModel.RefreshRecentCommand.Execute(null);
                _openWindow.Show();
                return;
            }
            openWindowViewModel.RefreshRecentCommand.Execute(null);
            openWindowViewModel.IsOpenWindowVisible = true;
            _openWindow.Focus();
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            IOpenWindowViewModel openWindowViewModel = ((ILapSelectionViewModel)DataContext).AddWindowViewModel;

            if (_addWindow == null)
            {
                openWindowViewModel.IsOpenWindowVisible = true;
                _addWindow = new OpenWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen, Owner = Window.GetWindow(this), DataContext = openWindowViewModel };
                _addWindow.Closed += AddWindowOnClosed;
                openWindowViewModel.RefreshRecentCommand.Execute(null);
                _addWindow.Show();
                return;
            }
            openWindowViewModel.RefreshRecentCommand.Execute(null);
            openWindowViewModel.IsOpenWindowVisible = true;
            _openWindow.Focus();
        }

        private void OpenWindowOnClosed(object sender, EventArgs e)
        {
            _openWindow.DataContext = null;
            _openWindow.Closed -= OpenWindowOnClosed;
            _openWindow = null;
        }

        private void AddWindowOnClosed(object sender, EventArgs e)
        {
            _addWindow.DataContext = null;
            _addWindow.Closed -= OpenWindowOnClosed;
            _addWindow = null;
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ISettingsWindowViewModel settingsWindowViewModel = ((ILapSelectionViewModel)DataContext).SettingsWindowViewModel;
            settingsWindowViewModel.OpenWindowCommand.Execute(null);

            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen, Owner = Window.GetWindow(this), DataContext = settingsWindowViewModel };
                _settingsWindow.Closed += SettingsWindowOnClosed;
                _settingsWindow.Show();
                return;
            }

            settingsWindowViewModel.IsWindowOpened = true;
            _settingsWindow.Focus();
        }

        private void SettingsWindowOnClosed(object sender, EventArgs e)
        {
            _settingsWindow.DataContext = null;
            _settingsWindow.Closed -= OpenWindowOnClosed;
            _settingsWindow = null;
        }
    }
}
