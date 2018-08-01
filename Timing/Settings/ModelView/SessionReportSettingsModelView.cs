namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Properties;
    using Model;

    public class SessionReportSettingsModelView : DependencyObject, INotifyPropertyChanged
    {
        private static readonly DependencyProperty ExportProperty = DependencyProperty.Register("Export", typeof(bool), typeof(SessionReportSettingsModelView), new PropertyMetadata(){ PropertyChangedCallback = PropertyChangedCallback});
        private static readonly DependencyProperty AutoOpenProperty = DependencyProperty.Register("AutoOpen", typeof(bool), typeof(SessionReportSettingsModelView), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Export
        {
            get => (bool)GetValue(ExportProperty);
            set => SetValue(ExportProperty, value);
        }

        public bool AutoOpen
        {
            get => (bool)GetValue(AutoOpenProperty);
            set => SetValue(AutoOpenProperty, value);
        }
        
        public static SessionReportSettingsModelView FromModel(SessionReportSettings model)
        {
            SessionReportSettingsModelView modelView =
                new SessionReportSettingsModelView() { Export = model.Export, AutoOpen = model.AutoOpen };
            return modelView;
        }

        public SessionReportSettings ToModel()
        {
            return new SessionReportSettings() { AutoOpen = AutoOpen, Export = Export, };
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SessionReportSettingsModelView sessionReportSettingsModelView)
            {
                sessionReportSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}