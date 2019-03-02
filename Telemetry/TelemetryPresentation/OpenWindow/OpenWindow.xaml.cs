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
using System.Windows.Shapes;

namespace SecondMonitor.TelemetryPresentation.OpenWindow
{
    using Telemetry.TelemetryApplication.ViewModels.OpenWindow;

    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
        }


        private void OpenWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*if (e.NewValue is IOpenWindowViewModel openWindowViewModel)
            {
                openWindowViewModel.RefreshRecentCommand.Execute(null);
            }*/
        }
    }
}
