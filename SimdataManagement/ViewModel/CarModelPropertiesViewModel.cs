namespace SecondMonitor.SimdataManagement.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using DataModel.BasicProperties;
    using DataModel.OperationalRange;
    using ViewModels;
    public class CarModelPropertiesViewModel : AbstractViewModel<CarModelProperties>
    {
        private Temperature _optimalBrakeTemperature;
        private Temperature _optimalBrakeTemperatureWindow;

        private string _carName;

        public CarModelPropertiesViewModel()
        {
            TyreCompoundsProperties = new ObservableCollection<TyreCompoundPropertiesViewModel>();
        }

        public string CarName
        {
            get => _carName;
            set
            {
                _carName = value;
                NotifyPropertyChanged();
            }
        }

        public Temperature OptimalBrakeTemperature
        {
            get => _optimalBrakeTemperature;
            set
            {
                _optimalBrakeTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public Temperature OptimalBrakeTemperatureWindow
        {
            get => _optimalBrakeTemperatureWindow;
            set
            {
                _optimalBrakeTemperatureWindow = value;
                NotifyPropertyChanged();
            }

        }

        public ObservableCollection<TyreCompoundPropertiesViewModel> TyreCompoundsProperties { get; }

        public override void FromModel(CarModelProperties model)
        {
            TyreCompoundsProperties.Clear();
            CarName = model.Name;
            OptimalBrakeTemperature = Temperature.FromCelsius(model.OptimalBrakeTemperature.InCelsius);
            OptimalBrakeTemperatureWindow = Temperature.FromCelsius(model.OptimalBrakeTemperatureWindow.InCelsius);
            foreach (TyreCompoundProperties modelTyreCompoundsProperty in model.TyreCompoundsProperties)
            {
                TyreCompoundPropertiesViewModel newViewModel = new TyreCompoundPropertiesViewModel();
                newViewModel.FromModel(modelTyreCompoundsProperty);
                TyreCompoundsProperties.Add(newViewModel);
            }
        }

        public override CarModelProperties SaveToNewModel()
        {
            return new CarModelProperties()
                       {
                           Name = CarName,
                           OptimalBrakeTemperature = Temperature.FromCelsius(OptimalBrakeTemperature.InCelsius),
                           OptimalBrakeTemperatureWindow = Temperature.FromCelsius(OptimalBrakeTemperatureWindow.InCelsius),
                           TyreCompoundsProperties = TyreCompoundsProperties.Select(x => x.SaveToNewModel()).ToList()
                           };
            }
        }
    }