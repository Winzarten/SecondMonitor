using SecondMonitor.WindowsControls.WPF.CarSettingsControl;

namespace SecondMonitor.SimdataManagement.ViewModel
{
    using DataModel.BasicProperties;
    using DataModel.OperationalRange;
    using ViewModels;
    public class TyreCompoundPropertiesViewModel : AbstractViewModel<TyreCompoundProperties>, ITyreSettingsViewModel
    {
        private string _compoundName;

        private Pressure _idealTyrePressure;
        private Pressure _idealTyrePressureWindow;

        private Temperature _idealTyreTemperature;
        private Temperature _idealTyreTemperatureWindow;

        private bool _isGlobalCompound;

        public bool IsGlobalCompound
        {
            get => _isGlobalCompound;
            set
            {
                _isGlobalCompound = value;
                NotifyPropertyChanged();
            }
        }

        public string CompoundName
        {
            get => _compoundName;
            set
            {
                _compoundName = value;
                NotifyPropertyChanged();
            }
        }

        public Pressure IdealTyrePressure
        {
            get => _idealTyrePressure;
            set
            {
                _idealTyrePressure = value;
                NotifyPropertyChanged();
            }
        }

        public Pressure IdealTyrePressureWindow
        {
            get => _idealTyrePressureWindow;
            set
            {
                _idealTyrePressureWindow = value;
                NotifyPropertyChanged();
            }

        }

        public Temperature IdealTyreTemperature
        {
            get => _idealTyreTemperature;
            set
            {
                _idealTyreTemperature = value;
                NotifyPropertyChanged();
            }
        }

        public Temperature IdealTyreTemperatureWindow
        {
            get => _idealTyreTemperatureWindow;
            set
            {
                _idealTyreTemperatureWindow = value;
                NotifyPropertyChanged();
            }
        }

        public override void FromModel(TyreCompoundProperties model)
        {
            CompoundName = model.CompoundName;
            IdealTyrePressure = Pressure.FromKiloPascals(model.IdealPressure.InKpa);
            IdealTyrePressureWindow = Pressure.FromKiloPascals(model.IdealPressureWindow.InKpa);

            IdealTyreTemperature = Temperature.FromCelsius(model.IdealTemperature.InCelsius);
            IdealTyreTemperatureWindow = Temperature.FromCelsius(model.IdealTemperatureWindow.InCelsius);
        }

        public override TyreCompoundProperties SaveToNewModel()
        {
            return new TyreCompoundProperties()
                       {
                           CompoundName = CompoundName,
                           IdealPressure = Pressure.FromKiloPascals(IdealTyrePressure.InKpa),
                           IdealPressureWindow = Pressure.FromKiloPascals(IdealTyrePressureWindow.InKpa),

                           IdealTemperature = Temperature.FromCelsius(IdealTyreTemperature.InCelsius),
                           IdealTemperatureWindow = Temperature.FromCelsius(IdealTyreTemperatureWindow.InCelsius),
                        };
        }
    }
}