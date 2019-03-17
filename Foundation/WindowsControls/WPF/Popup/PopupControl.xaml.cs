using System.Windows.Controls;

namespace SecondMonitor.WindowsControls.WPF.Popup
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for PopupControl.xaml
    /// </summary>
    public partial class PopupControl : UserControl
    {
        public static readonly DependencyProperty ElementToHideProperty = DependencyProperty.Register("ElementToHide", typeof(FrameworkElement), typeof(PopupControl), new PropertyMetadata(default(FrameworkElement)));
        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(PopupControl), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty PopupWindowTitleProperty = DependencyProperty.Register("PopupWindowTitle", typeof(string), typeof(PopupControl), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PopupWindowTemplateProperty = DependencyProperty.Register("PopupWindowTemplate", typeof(DataTemplate), typeof(PopupControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate PopupWindowTemplate
        {
            get => (DataTemplate) GetValue(PopupWindowTemplateProperty);
            set => SetValue(PopupWindowTemplateProperty, value);
        }

        public string PopupWindowTitle
        {
            get => (string) GetValue(PopupWindowTitleProperty);
            set => SetValue(PopupWindowTitleProperty, value);
        }

        public object PopupContent
        {
            get => (object) GetValue(PopupContentProperty);
            set => SetValue(PopupContentProperty, value);
        }

        private PopUpWindow _popUpWindow;

        public FrameworkElement ElementToHide
        {
            get => (FrameworkElement) GetValue(ElementToHideProperty);
            set => SetValue(ElementToHideProperty, value);
        }

        public PopupControl()
        {
            InitializeComponent();
        }

        private void OnPopupButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ElementToHide == null)
            {
                return;
            }

            _popUpWindow = new PopUpWindow {Title = PopupWindowTitle, Width = ElementToHide.ActualWidth, Height = ElementToHide.ActualHeight};
            _popUpWindow.Closed += PopUpWindowOnClosed;
            _popUpWindow.Content = PopupContent;
            _popUpWindow.DataContext = PopupContent;
            _popUpWindow.ContentTemplate = PopupWindowTemplate;
            _popUpWindow.Show();

            ElementToHide.Visibility = Visibility.Collapsed;
            ElementToHide.IsEnabled = false;
        }

        private void PopUpWindowOnClosed(object sender, EventArgs e)
        {
            _popUpWindow.Closed -= PopUpWindowOnClosed;
            _popUpWindow.Content = null;
            _popUpWindow.DataContext = null;
            _popUpWindow = null;

            ElementToHide.Visibility = Visibility.Visible;
            ElementToHide.IsEnabled = true;
        }
    }
}
