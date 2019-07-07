namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using Model;

    public class ColumnsSettingsViewModel : AbstractViewModel<ColumnsSettings>
    {
        private ColumnSettingsViewModel _carClassName;
        public ColumnSettingsViewModel CarClassName
        {
            get => _carClassName;
            set => SetProperty(ref _carClassName, value);
        }

        private ColumnSettingsViewModel _position;
        public ColumnSettingsViewModel Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        private ColumnSettingsViewModel _name;
        public ColumnSettingsViewModel Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private ColumnSettingsViewModel _carName;
        public ColumnSettingsViewModel CarName
        {
            get => _carName;
            set => SetProperty(ref _carName, value);
        }

        private ColumnSettingsViewModel _completedLaps;
        public ColumnSettingsViewModel CompletedLaps
        {
            get => _completedLaps;
            set => SetProperty(ref _completedLaps, value);
        }

        private ColumnSettingsViewModel _lastLapTime;
        public ColumnSettingsViewModel LastLapTime
        {
            get => _lastLapTime;
            set => SetProperty(ref _lastLapTime, value);
        }

        private ColumnSettingsViewModel _pace;
        public ColumnSettingsViewModel Pace
        {
            get => _pace;
            set => SetProperty(ref _pace, value);
        }

        private ColumnSettingsViewModel _bestLap;
        public ColumnSettingsViewModel BestLap
        {
            get => _bestLap;
            set => SetProperty(ref _bestLap, value);
        }

        private ColumnSettingsViewModel _currentLapProgressTime;
        public ColumnSettingsViewModel CurrentLapProgressTime
        {
            get => _currentLapProgressTime;
            set => SetProperty(ref _currentLapProgressTime, value);
        }

        private ColumnSettingsViewModel _lastPitInfo;
        public ColumnSettingsViewModel LastPitInfo
        {
            get => _lastPitInfo;
            set => SetProperty(ref _lastPitInfo, value);
        }

        private ColumnSettingsViewModel _timeToPlayer;
        public ColumnSettingsViewModel TimeToPlayer
        {
            get => _timeToPlayer;
            set => SetProperty(ref _timeToPlayer, value);
        }

        private ColumnSettingsViewModel _topSpeed;
        public ColumnSettingsViewModel TopSpeed
        {
            get => _topSpeed;
            set => SetProperty(ref _topSpeed, value);
        }

        private ColumnSettingsViewModel _sector1;
        public ColumnSettingsViewModel Sector1
        {
            get => _sector1;
            set => SetProperty(ref _sector1, value);
        }

        private ColumnSettingsViewModel _sector2;
        public ColumnSettingsViewModel Sector2
        {
            get => _sector2;
            set => SetProperty(ref _sector2, value);
        }

        private ColumnSettingsViewModel _sector3;
        public ColumnSettingsViewModel Sector3
        {
            get => _sector3;
            set => SetProperty(ref _sector3, value);
        }

        private ColumnSettingsViewModel _rating;
        public ColumnSettingsViewModel Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public static ColumnsSettingsViewModel CreateFromModel(ColumnsSettings columnsSettings)
        {
            ColumnsSettingsViewModel newColumnSettingsViewModel = new ColumnsSettingsViewModel();
            newColumnSettingsViewModel.ApplyModel(columnsSettings);
            return newColumnSettingsViewModel;
        }


        protected override void ApplyModel(ColumnsSettings columnsSettings)
        {
            Position = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Position);
            Name = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Name);
            CarName = ColumnSettingsViewModel.CreateFromModel(columnsSettings.CarName);
            CarClassName = ColumnSettingsViewModel.CreateFromModel(columnsSettings.CarClassName);
            CompletedLaps = ColumnSettingsViewModel.CreateFromModel(columnsSettings.CompletedLaps);
            LastLapTime = ColumnSettingsViewModel.CreateFromModel(columnsSettings.LastLapTime);
            Pace = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Pace);
            BestLap = ColumnSettingsViewModel.CreateFromModel(columnsSettings.BestLap);
            CurrentLapProgressTime = ColumnSettingsViewModel.CreateFromModel(columnsSettings.CurrentLapProgressTime);
            LastPitInfo = ColumnSettingsViewModel.CreateFromModel(columnsSettings.LastPitInfo);
            TimeToPlayer = ColumnSettingsViewModel.CreateFromModel(columnsSettings.TimeToPlayer);
            TopSpeed = ColumnSettingsViewModel.CreateFromModel(columnsSettings.TopSpeed);
            Sector1 = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Sector1);
            Sector2 = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Sector2);
            Sector3 = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Sector3);
            Rating = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Rating);
        }

        public override ColumnsSettings SaveToNewModel()
        {
            return new ColumnsSettings
            {
                Position = Position.ToModel(),
                Name = Name.ToModel(),
                CarName = CarName.ToModel(),
                CarClassName = CarClassName.ToModel(),
                CompletedLaps = CompletedLaps.ToModel(),
                LastLapTime = LastLapTime.ToModel(),
                Pace = Pace.ToModel(),
                BestLap = BestLap.ToModel(),
                CurrentLapProgressTime = CurrentLapProgressTime.ToModel(),
                LastPitInfo = LastPitInfo.ToModel(),
                TimeToPlayer = TimeToPlayer.ToModel(),
                TopSpeed = TopSpeed.ToModel(),
                Sector1 = Sector1.ToModel(),
                Sector2 = Sector2.ToModel(),
                Sector3 = Sector3.ToModel(),
                Rating = Rating.ToModel(),
            };
        }
    }
}