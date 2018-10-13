namespace SecondMonitor.Timing.Settings.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Properties;
    using Model;

    public class SessionReportSettingsViewModel : DependencyObject, INotifyPropertyChanged
    {
        private static readonly DependencyProperty ExportProperty = DependencyProperty.Register("Export", typeof(bool), typeof(SessionReportSettingsViewModel), new PropertyMetadata(){ PropertyChangedCallback = PropertyChangedCallback});
        private static readonly DependencyProperty AutoOpenProperty = DependencyProperty.Register("AutoOpen", typeof(bool), typeof(SessionReportSettingsViewModel), new PropertyMetadata() { PropertyChangedCallback = PropertyChangedCallback });

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

        public static SessionReportSettingsViewModel FromModel(SessionReportSettings model)
        {
            SessionReportSettingsViewModel viewModel =
                new SessionReportSettingsViewModel() { Export = model.Export, AutoOpen = model.AutoOpen };
            return viewModel;
        }

        public SessionReportSettings ToModel()
        {
            return new SessionReportSettings() { AutoOpen = AutoOpen, Export = Export, };
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SessionReportSettingsViewModel sessionReportSettingsModelView)
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