using System.Diagnostics;
using System.Windows.Navigation;

namespace SecondMonitor.Timing.Presentation.View
{
    using System;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for TimingGUI.xaml
    /// </summary>
    public partial class TimingGui : Window
    {

        public TimingGui()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource != null)
            {
                hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            }

            base.OnSourceInitialized(e);
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
