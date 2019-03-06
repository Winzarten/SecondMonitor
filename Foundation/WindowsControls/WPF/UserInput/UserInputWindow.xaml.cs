namespace SecondMonitor.WindowsControls.WPF.UserInput
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for UserInputWindow.xaml
    /// </summary>
    public partial class UserInputWindow : Window
    {
        public UserInputWindow()
        {
            InitializeComponent();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
