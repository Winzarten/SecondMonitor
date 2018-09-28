namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.Timing.Properties;
    using SecondMonitor.Timing.Settings.Model;

    public class SessionOptionsViewModel : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty TimesDisplayModeProperty = DependencyProperty.Register("TimesDisplayMode", typeof(DisplayModeEnum), typeof(SessionOptionsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty OrderingModeProperty = DependencyProperty.Register("OrderingMode", typeof(DisplayModeEnum), typeof(SessionOptionsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty SessionNameProperty = DependencyProperty.Register("SessionName", typeof(string), typeof(SessionOptionsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ColumnsSettingsViewProperty = DependencyProperty.Register("ColumnsSettingsView", typeof(ColumnsSettingsViewModel), typeof(SessionOptionsViewModel), new PropertyMetadata{ PropertyChangedCallback = PropertyChangedCallback});

        public DisplayModeEnum TimesDisplayMode
        {
            get => (DisplayModeEnum)GetValue(TimesDisplayModeProperty);
            set => SetValue(TimesDisplayModeProperty, value);
        }

        public DisplayModeEnum OrderingMode
        {
            get => (DisplayModeEnum)GetValue(OrderingModeProperty);
            set => SetValue(OrderingModeProperty, value);
        }

        public string SessionName
        {
            get => (string)GetValue(SessionNameProperty);
            set => SetValue(SessionNameProperty, value);
        }

        public ColumnsSettingsViewModel ColumnsSettingsView
        {
            get => (ColumnsSettingsViewModel)GetValue(ColumnsSettingsViewProperty);
            set => SetValue(ColumnsSettingsViewProperty, value);
        }

        public void FromModel(SessionOptions model)
        {
            TimesDisplayMode = model.TimesDisplayMode;
            OrderingMode = model.OrderingDisplayMode;
            SessionName = model.SessionName;
            ColumnsSettingsView = ColumnsSettingsViewModel.CreateFromModel(model.ColumnsSettings);
        }

        public SessionOptions ToModel()
        {
            return new SessionOptions()
            {
                OrderingDisplayMode = OrderingMode,
                TimesDisplayMode = TimesDisplayMode,
                SessionName = SessionName,
                ColumnsSettings = ColumnsSettingsView.ToModel()
            };

        }

        public static SessionOptionsViewModel CreateFromModel(SessionOptions model)
        {
            SessionOptionsViewModel newViewModel = new SessionOptionsViewModel();
            newViewModel.FromModel(model);
            return newViewModel;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SessionOptionsViewModel sender = (SessionOptionsViewModel)dependencyObject;
            sender.OnPropertyChanged(dependencyPropertyChangedEventArgs.Property.Name);
        }

    }
}
