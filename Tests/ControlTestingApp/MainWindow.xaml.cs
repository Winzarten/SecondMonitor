namespace ControlTestingApp
{
    using System;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
    }
}
