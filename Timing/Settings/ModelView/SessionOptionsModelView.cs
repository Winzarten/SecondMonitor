namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.Timing.Properties;
    using SecondMonitor.Timing.Settings.Model;

    public class SessionOptionsModelView : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty TimesDisplayModeProperty = DependencyProperty.Register("TimesDisplayMode", typeof(DisplayModeEnum), typeof(SessionOptionsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty OrderingModeProperty = DependencyProperty.Register("OrderingMode", typeof(DisplayModeEnum), typeof(SessionOptionsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty SessionNameProperty = DependencyProperty.Register("SessionName", typeof(string), typeof(SessionOptionsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty ColumnsSettingsProperty = DependencyProperty.Register("ColumnsSettings", typeof(ColumnsSettingsModelView), typeof(SessionOptionsModelView), new PropertyMetadata{ PropertyChangedCallback = PropertyChangedCallback});

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

        public ColumnsSettingsModelView ColumnsSettings
        {
            get => (ColumnsSettingsModelView)GetValue(ColumnsSettingsProperty);
            set => SetValue(ColumnsSettingsProperty, value);
        }

        public void FromModel(SessionOptions model)
        {
            TimesDisplayMode = model.TimesDisplayMode;
            OrderingMode = model.OrderingDisplayMode;
            SessionName = model.SessionName;
            ColumnsSettings = ColumnsSettingsModelView.CreateFromModel(model.ColumnsSettings);
        }

        public SessionOptions ToModel()
        {
            return new SessionOptions()
            {
                OrderingDisplayMode = OrderingMode,
                TimesDisplayMode = TimesDisplayMode,
                SessionName = SessionName,
                ColumnsSettings = ColumnsSettings.ToModel()
            };

        }

        public static SessionOptionsModelView CreateFromModel(SessionOptions model)
        {
            SessionOptionsModelView newModelView = new SessionOptionsModelView();
            newModelView.FromModel(model);
            return newModelView;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SessionOptionsModelView sender = (SessionOptionsModelView)dependencyObject;
            sender.OnPropertyChanged(dependencyPropertyChangedEventArgs.Property.Name);
        }

    }
}
