using System.Collections.ObjectModel;
using SecondMonitor.WindowsControls.WPF.CarSettingsControl;

namespace SecondMonitor.Timing.CarSettings
{
    using System.Windows.Input;

    using DataModel.BasicProperties;
    using SimdataManagement.ViewModel;
    using Settings.ViewModel;
    using ViewModels;

    public class CarSettingsWindowViewModel : AbstractViewModel
    {
        private readonly DisplaySettingsViewModel _displaySettingsViewModel;

        private ICommand _cancelButtonCommand;
        private ICommand _okButtonCommand;
        private ICommand _copyCompoundToLocalCommand;
        private ObservableCollection<TyreCompoundPropertiesViewModel> _tyreSettingsViewModels;
        private TyreCompoundPropertiesViewModel _selectedTyreSettingsViewModel;


        private CarModelPropertiesViewModel _carModelPropertiesViewModel;

        public CarSettingsWindowViewModel(DisplaySettingsViewModel _displaySettingsViewModel)
        {
            this._displaySettingsViewModel = _displaySettingsViewModel;
        }

        public PressureUnits PressureUnits => _displaySettingsViewModel.PressureUnits;

        public TemperatureUnits TemperatureUnits => _displaySettingsViewModel.TemperatureUnits;

        public ObservableCollection<TyreCompoundPropertiesViewModel> TyreSettingsViewModels
        {
            get => _tyreSettingsViewModels;
            set
            {
                _tyreSettingsViewModels = value;
                NotifyPropertyChanged();
            }
        }

        public TyreCompoundPropertiesViewModel SelectedTyreSettingsViewModel
        {
            get => _selectedTyreSettingsViewModel;
            set
            {
                _selectedTyreSettingsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public CarModelPropertiesViewModel CarModelPropertiesViewModel
        {
            get => _carModelPropertiesViewModel;
            set
            {
                _carModelPropertiesViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand CancelButtonCommand
        {
            get => _cancelButtonCommand;
            set
            {
                _cancelButtonCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand OkButtonCommand
        {
            get => _okButtonCommand;
            set
            {
                _okButtonCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand CopyCompoundToLocalCommand
        {
            get => _copyCompoundToLocalCommand;
            set
            {
                _copyCompoundToLocalCommand = value;
                NotifyPropertyChanged();
            }
        }
    }
}