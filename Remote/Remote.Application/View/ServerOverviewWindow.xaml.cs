using System.Windows;

namespace SecondMonitor.Remote.Application.View
{
    using System;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ServerOverviewWindow.xaml
    /// </summary>
    public partial class ServerOverviewWindow : Window
    {
        public ServerOverviewWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ImageSource imageSource = this.Icon;
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource == null)
            {
                return;
            }

            ni.Icon = System.Drawing.Icon.FromHandle(hwndSource.Handle);
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            base.OnSourceInitialized(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }
    }
}
