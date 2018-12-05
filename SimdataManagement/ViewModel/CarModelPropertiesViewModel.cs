namespace SecondMonitor.SimdataManagement.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using DataModel.BasicProperties;
    using DataModel.OperationalRange;
    using ViewModels;
    public class CarModelPropertiesViewModel : AbstractViewModel<CarModelProperties>
    {
        private Temperature _minimalOptimalBrakeTemperature;
        private Temperature _maximumOptimalBrakeTemperature;

        private string _carName;
        private int _wheelRotation;

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

        public int WheelRotation
        {
            get => _wheelRotation;
            set
            {
                _wheelRotation = value;
                NotifyPropertyChanged();
            }
        }

        public Temperature MinimalOptimalBrakeTemperature
        {
            get => _minimalOptimalBrakeTemperature;
            set
            {
                _minimalOptimalBrakeTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public Temperature MaximumOptimalBrakeTemperature
        {
            get => _maximumOptimalBrakeTemperature;
            set
            {
                _maximumOptimalBrakeTemperature = value;
                NotifyPropertyChanged();
            }

        }

        public ObservableCollection<TyreCompoundPropertiesViewModel> TyreCompoundsProperties { get; }

        public override void FromModel(CarModelProperties model)
        {
            TyreCompoundsProperties.Clear();
            CarName = model.Name;
            WheelRotation = model.WheelRotation;
            MinimalOptimalBrakeTemperature = Temperature.FromCelsius(model.OptimalBrakeTemperature.InCelsius - (model.OptimalBrakeTemperatureWindow.InCelsius));
            MaximumOptimalBrakeTemperature = Temperature.FromCelsius(model.OptimalBrakeTemperature.InCelsius + (model.OptimalBrakeTemperatureWindow.InCelsius));
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
                           OptimalBrakeTemperature = Temperature.FromCelsius((MinimalOptimalBrakeTemperature.InCelsius + MaximumOptimalBrakeTemperature.InCelsius) * 0.5),
                           OptimalBrakeTemperatureWindow = Temperature.FromCelsius((MaximumOptimalBrakeTemperature.InCelsius - MinimalOptimalBrakeTemperature.InCelsius) * 0.5),
                           TyreCompoundsProperties = TyreCompoundsProperties.Select(x => x.SaveToNewModel()).ToList(),
                           WheelRotation = WheelRotation
                           };
            }
        }
    }