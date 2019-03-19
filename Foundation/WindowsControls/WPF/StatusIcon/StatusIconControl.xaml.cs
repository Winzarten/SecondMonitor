using System.Windows.Controls;

namespace SecondMonitor.WindowsControls.WPF.StatusIcon
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for StatusIconControl.xaml
    /// </summary>
    public partial class StatusIconControl : UserControl
    {
        public static readonly DependencyProperty StatusIconProperty = DependencyProperty.Register("StatusIcon", typeof(Viewbox), typeof(StatusIconControl), new PropertyMetadata(default(Viewbox)));

        public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(StatusIconControl), new PropertyMetadata(new Thickness(5)));

        public static readonly DependencyProperty IconMaxWidthProperty = DependencyProperty.Register("IconMaxWidth", typeof(double), typeof(StatusIconControl), new PropertyMetadata(default(double)));

        public double IconMaxWidth
        {
            get => (double) GetValue(IconMaxWidthProperty);
            set => SetValue(IconMaxWidthProperty, value);
        }

        public Thickness IconMargin
        {
            get => (Thickness) GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

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
