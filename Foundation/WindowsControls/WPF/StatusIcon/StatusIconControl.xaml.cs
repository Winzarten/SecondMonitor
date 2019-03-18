using System.Windows.Controls;

namespace SecondMonitor.WindowsControls.WPF.StatusIcon
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for StatusIconControl.xaml
    /// </summary>
    public partial class StatusIconControl : UserControl
    {
        public static readonly DependencyProperty StatusIconProperty = DependencyProperty.Register(
            "StatusIcon", typeof(Viewbox), typeof(StatusIconControl), new PropertyMetadata(default(Viewbox)));

        public Viewbox StatusIcon
        {
            get => (Viewbox) GetValue(StatusIconProperty);
            set => SetValue(StatusIconProperty, value);
        }

        public StatusIconControl()
        {
            InitializeComponent();
        }
    }
}
