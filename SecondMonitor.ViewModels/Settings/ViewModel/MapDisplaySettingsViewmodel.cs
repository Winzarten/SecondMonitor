namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using Model;
    using ViewModels;

    public class MapDisplaySettingsViewModel : AbstractViewModel<MapDisplaySettings>
    {
        private bool _autosScaleDrivers;
        private bool _keepMapRatio;


        public bool AutosScaleDrivers
        {
            get => _autosScaleDrivers;
            set
            {
                _autosScaleDrivers = value;
                NotifyPropertyChanged();
            }
        }

        public bool KeepMapRatio
        {
            get => _keepMapRatio;
            set
            {
                _keepMapRatio = value;
                NotifyPropertyChanged();
            }
        }

        public override void FromModel(MapDisplaySettings model)
        {
            KeepMapRatio = model.KeepMapRatio;
            AutosScaleDrivers = model.AutosScaleDrivers;
        }

        public override MapDisplaySettings SaveToNewModel()
        {
            return new MapDisplaySettings()
            {
                AutosScaleDrivers = AutosScaleDrivers,
                KeepMapRatio = KeepMapRatio,
            };
        }
    }
}