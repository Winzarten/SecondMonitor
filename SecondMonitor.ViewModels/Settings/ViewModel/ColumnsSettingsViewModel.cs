namespace SecondMonitor.ViewModels.Settings.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Model;
    using Properties;

    public class ColumnsSettingsViewModel : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CompletedLapsProperty = DependencyProperty.Register("CompletedLaps", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty LastLapTimeProperty = DependencyProperty.Register("LastLapTime", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty PaceProperty = DependencyProperty.Register("Pace", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty BestLapProperty = DependencyProperty.Register("BestLap", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CurrentLapProgressTimeProperty = DependencyProperty.Register("CurrentLapProgressTime", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty LastPitInfoProperty = DependencyProperty.Register("LastPitInfo", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty TimeToPlayerProperty = DependencyProperty.Register("TimeToPlayer", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty TopSpeedProperty = DependencyProperty.Register("TopSpeed", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector1Property = DependencyProperty.Register("Sector1", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector2Property = DependencyProperty.Register("Sector2", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector3Property = DependencyProperty.Register("Sector3", typeof(ColumnSettingsViewModel), typeof(ColumnsSettingsViewModel), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

        public event PropertyChangedEventHandler PropertyChanged;

        public ColumnSettingsViewModel Position
        {
            get => (ColumnSettingsViewModel)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public ColumnSettingsViewModel Name
        {
            get => (ColumnSettingsViewModel)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public ColumnSettingsViewModel CarName
        {
            get => (ColumnSettingsViewModel)GetValue(CarNameProperty);
            set => SetValue(CarNameProperty, value);
        }

        public ColumnSettingsViewModel CompletedLaps
        {
            get => (ColumnSettingsViewModel)GetValue(CompletedLapsProperty);
            set => SetValue(CompletedLapsProperty, value);
        }

        public ColumnSettingsViewModel LastLapTime
        {
            get => (ColumnSettingsViewModel)GetValue(LastLapTimeProperty);
            set => SetValue(LastLapTimeProperty, value);
        }

        public ColumnSettingsViewModel Pace
        {
            get => (ColumnSettingsViewModel)GetValue(PaceProperty);
            set => SetValue(PaceProperty, value);
        }

        public ColumnSettingsViewModel BestLap
        {
            get => (ColumnSettingsViewModel)GetValue(BestLapProperty);
            set => SetValue(BestLapProperty, value);
        }

        public ColumnSettingsViewModel CurrentLapProgressTime
        {
            get => (ColumnSettingsViewModel)GetValue(CurrentLapProgressTimeProperty);
            set => SetValue(CurrentLapProgressTimeProperty, value);
        }

        public ColumnSettingsViewModel LastPitInfo
        {
            get => (ColumnSettingsViewModel)GetValue(LastPitInfoProperty);
            set => SetValue(LastPitInfoProperty, value);
        }

        public ColumnSettingsViewModel TimeToPlayer
        {
            get => (ColumnSettingsViewModel)GetValue(TimeToPlayerProperty);
            set => SetValue(TimeToPlayerProperty, value);
        }

        public ColumnSettingsViewModel TopSpeed
        {
            get => (ColumnSettingsViewModel)GetValue(TopSpeedProperty);
            set => SetValue(TopSpeedProperty, value);
        }

        public ColumnSettingsViewModel Sector1
        {
            get => (ColumnSettingsViewModel)GetValue(Sector1Property);
            set => SetValue(Sector1Property, value);
        }

        public ColumnSettingsViewModel Sector2
        {
            get => (ColumnSettingsViewModel)GetValue(Sector2Property);
            set => SetValue(Sector2Property, value);
        }

        public ColumnSettingsViewModel Sector3
        {
            get => (ColumnSettingsViewModel)GetValue(Sector3Property);
            set => SetValue(Sector3Property, value);
        }

        public void FromModel(ColumnsSettings columnsSettings)
        {
            Position = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Position);
            Name = ColumnSettingsViewModel.CreateFromModel(columnsSettings.Name);
            CarName = ColumnSettingsViewModel.CreateFromModel(columnsSettings.CarName);
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
        }

        public static ColumnsSettingsViewModel CreateFromModel(ColumnsSettings columnsSettings)
        {
            ColumnsSettingsViewModel newColumnSettingsViewModel = new ColumnsSettingsViewModel();
            newColumnSettingsViewModel.FromModel(columnsSettings);
            return newColumnSettingsViewModel;
        }

        public ColumnsSettings ToModel()
        {
            return new ColumnsSettings
                       {
                           Position = Position.ToModel(),
                           Name = Name.ToModel(),
                           CarName = CarName.ToModel(),
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
                           Sector3 = Sector3.ToModel()
        };

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColumnsSettingsViewModel columnsSettingsModelView)
            {
                columnsSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }
    }
}