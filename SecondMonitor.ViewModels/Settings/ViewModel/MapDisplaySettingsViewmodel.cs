namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using Model;
    using ViewModels;

    public class MapDisplaySettingsViewModel : AbstractViewModel<MapDisplaySettings>
    {
        private bool _autoScaleDrivers;
        private bool _keepMapRatio;
        private int _mapPointsInterval;
        private bool _alwaysUseCirce;


        public bool AutoScaleDrivers
        {
            get => _autoScaleDrivers;
            set
            {
                _autoScaleDrivers = value;
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

        public bool AlwaysUseCirce
        {
            get => _alwaysUseCirce;
            set
            {
                _alwaysUseCirce = value;
                NotifyPropertyChanged();
            }
        }

        public int MapPointsInterval
        {
            get => _mapPointsInterval;
            set
            {
                _mapPointsInterval = value;
                NotifyPropertyChanged();
            }
        }

        public override void FromModel(MapDisplaySettings model)
        {
            KeepMapRatio = model.KeepMapRatio;
            AutoScaleDrivers = model.AutosScaleDrivers;
            MapPointsInterval = model.MapPointsInterval;
        }

        public override MapDisplaySettings SaveToNewModel()
        {
            return new MapDisplaySettings()
            {
                AutosScaleDrivers = AutoScaleDrivers,
                KeepMapRatio = KeepMapRatio,
                MapPointsInterval = MapPointsInterval
            };
        }
    }
}