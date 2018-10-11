namespace SecondMonitor.SimdataManagement.SimSettings
{
    using System.Collections.Generic;
    using System.IO;

    using SecondMonitor.DataModel.OperationalRange;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Snapshot.Systems;
    using SecondMonitor.PluginManager.Visitor;

    public class SimSettingAdapter : IDataSetVisitor
    {
        private readonly SimSettingsLoader _simSettingsLoader;
        private DataSourceProperties _dataSourceProperties;

        private KeyValuePair<string, CarModelProperties> _lastCarProperties;
        private KeyValuePair<string, TyreCompoundProperties> _lastCompound;

        public SimSettingAdapter(string userConfigPath)
        {
            string execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string configPath = Path.Combine(execPath, "Config");
            _simSettingsLoader = new SimSettingsLoader(configPath, userConfigPath);
        }

        public SimSettingAdapter(string baseConfigPath, string userConfigPath)
        {
            _simSettingsLoader = new SimSettingsLoader(baseConfigPath, userConfigPath);
        }

        public string UserConfigPath
        {
            get => _simSettingsLoader.OverridingPath;
            set => _simSettingsLoader.OverridingPath = value;
        }

        public void Visit(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet?.PlayerInfo?.CarName == null)
            {
                return;
            }

            if (_dataSourceProperties?.SourceName != simulatorDataSet.Source)
            {
                ReloadDataSourceProperties(simulatorDataSet.Source);
            }

            CarModelProperties carModel = GetCarModelProperties(simulatorDataSet);
            ApplyCarMode(simulatorDataSet, carModel);
        }

        private void ApplyCarMode(SimulatorDataSet simulatorDataSet, CarModelProperties carModel)
        {
            ApplyWheelProperty(simulatorDataSet, simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft, carModel);
            ApplyWheelProperty(simulatorDataSet, simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontRight, carModel);
            ApplyWheelProperty(simulatorDataSet, simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearLeft, carModel);
            ApplyWheelProperty(simulatorDataSet, simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.RearRight, carModel);
        }

        private void ApplyWheelProperty(SimulatorDataSet simulatorDataSet, WheelInfo wheelInfo, CarModelProperties carModel)
        {
            wheelInfo.BrakeTemperature.IdealQuantity.InCelsius = carModel.OptimalBrakeTemperature.InCelsius;
            wheelInfo.BrakeTemperature.IdealQuantityWindow.InCelsius = carModel.OptimalBrakeTemperatureWindow.InCelsius;

            if (string.IsNullOrWhiteSpace(wheelInfo.TyreType))
            {
                return;
            }

            TyreCompoundProperties tyreCompound = GetTyreCompound(simulatorDataSet, wheelInfo, carModel);

            wheelInfo.TyrePressure.IdealQuantity.InKpa = tyreCompound.IdealPressure.InKpa;
            wheelInfo.TyrePressure.IdealQuantityWindow.InKpa = tyreCompound.IdealPressureWindow.InKpa;

            wheelInfo.LeftTyreTemp.IdealQuantity.InCelsius = tyreCompound.IdealTemperature.InCelsius;
            wheelInfo.LeftTyreTemp.IdealQuantityWindow.InCelsius = tyreCompound.IdealTemperatureWindow.InCelsius;

            wheelInfo.RightTyreTemp.IdealQuantity.InCelsius = tyreCompound.IdealTemperature.InCelsius;
            wheelInfo.RightTyreTemp.IdealQuantityWindow.InCelsius = tyreCompound.IdealTemperatureWindow.InCelsius;

            wheelInfo.CenterTyreTemp.IdealQuantity.InCelsius = tyreCompound.IdealTemperature.InCelsius;
            wheelInfo.CenterTyreTemp.IdealQuantityWindow.InCelsius = tyreCompound.IdealTemperatureWindow.InCelsius;

            wheelInfo.TyreCoreTemperature.IdealQuantity.InCelsius = tyreCompound.IdealTemperature.InCelsius;
            wheelInfo.TyreCoreTemperature.IdealQuantityWindow.InCelsius = tyreCompound.IdealTemperatureWindow.InCelsius;
        }

        private TyreCompoundProperties GetTyreCompound(SimulatorDataSet simulatorDataSet, WheelInfo wheelInfo, CarModelProperties carModel)
        {
            string compoundName = wheelInfo.TyreType;
            if (_lastCompound.Key == compoundName)
            {
                return _lastCompound.Value;
            }

            TyreCompoundProperties tyreCompound = null;

            if (!simulatorDataSet.SimulatorSourceInfo.GlobalTyreCompounds)
            {
                tyreCompound = carModel.GetTyreCompound(compoundName);
                if (tyreCompound != null)
                {
                    _lastCompound = new KeyValuePair<string, TyreCompoundProperties>(tyreCompound.CompoundName, tyreCompound);
                    return tyreCompound;
                }
            }

            tyreCompound = _dataSourceProperties.GetTyreCompound(compoundName);
            if (tyreCompound == null)
            {
                tyreCompound = CreateTyreCompound(wheelInfo);
                if (simulatorDataSet.SimulatorSourceInfo.GlobalTyreCompounds)
                {
                    _dataSourceProperties.AddTyreCompound(tyreCompound);
                }
                else
                {
                    carModel.AddTyreCompound(tyreCompound);
                }
                _simSettingsLoader.SaveDataSourceProperties(_dataSourceProperties);
            }

            _lastCompound = new KeyValuePair<string, TyreCompoundProperties>(tyreCompound.CompoundName, tyreCompound);
            return tyreCompound;

        }

        private CarModelProperties GetCarModelProperties(SimulatorDataSet simulatorDataSet)
        {
            string carName = simulatorDataSet.PlayerInfo.CarName;

            if (carName == _lastCarProperties.Key)
            {
                return _lastCarProperties.Value;
            }

            CarModelProperties carModelProperties = _dataSourceProperties.GetCarModel(carName);

            if (carModelProperties == null)
            {
                carModelProperties = CreateNewCarModelProperties(carName, simulatorDataSet);
            }

            _lastCarProperties = new KeyValuePair<string, CarModelProperties>(carModelProperties.Name, carModelProperties);
            return carModelProperties;
        }

        private CarModelProperties CreateNewCarModelProperties(string carName, SimulatorDataSet simulatorDataSet)
        {
            CarModelProperties newCarModelProperties = new CarModelProperties() { Name = carName };
            newCarModelProperties.OptimalBrakeTemperature = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.IdealQuantity;
            newCarModelProperties.OptimalBrakeTemperatureWindow = simulatorDataSet.PlayerInfo.CarInfo.WheelsInfo.FrontLeft.BrakeTemperature.IdealQuantityWindow;
            _dataSourceProperties.AddCarModel(newCarModelProperties);
            _simSettingsLoader.SaveDataSourceProperties(_dataSourceProperties);
            return newCarModelProperties;
        }

        private TyreCompoundProperties CreateTyreCompound(WheelInfo wheel)
        {
            TyreCompoundProperties tyreCompound = new TyreCompoundProperties() { CompoundName = wheel.TyreType };
            tyreCompound.IdealPressure = wheel.TyrePressure.IdealQuantity;
            tyreCompound.IdealPressureWindow = wheel.TyrePressure.IdealQuantityWindow;

            tyreCompound.IdealTemperature = wheel.CenterTyreTemp.IdealQuantity;
            tyreCompound.IdealTemperatureWindow = wheel.CenterTyreTemp.IdealQuantityWindow;

            return tyreCompound;
        }

        private void ReloadDataSourceProperties(string sourceName)
        {
            _dataSourceProperties = _simSettingsLoader.GetDataSourcePropertiesAsync(sourceName);
        }
    }
}