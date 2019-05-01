namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Model;
    using Properties;

    public class SessionOptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DisplayModeEnum _timesDisplayMode;
        private DisplayModeEnum _orderingMode;
        private string _sessionName;
        private ColumnsSettingsViewModel _columnsSettingsViewModel;

        public DisplayModeEnum TimesDisplayMode
        {
            get => _timesDisplayMode;
            set
            {
                _timesDisplayMode = value;
                OnPropertyChanged();
            }
        }

        public DisplayModeEnum OrderingMode
        {
            get => _orderingMode;
            set
            {
                _orderingMode = value;
                OnPropertyChanged();
            }
        }

        public string SessionName
        {
            get => _sessionName;
            set
            {
                _sessionName = value;
                OnPropertyChanged();
            }
        }

        public ColumnsSettingsViewModel ColumnsSettingsView
        {
            get => _columnsSettingsViewModel;
            set
            {
                _columnsSettingsViewModel = value;
                OnPropertyChanged();
            }
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
                ColumnsSettings = ColumnsSettingsView.SaveToNewModel()
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
    }
}
