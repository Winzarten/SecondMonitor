namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Properties;
    using Model;

    public class ColumnsSettingsModelView : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CarNameProperty = DependencyProperty.Register("CarName", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CompletedLapsProperty = DependencyProperty.Register("CompletedLaps", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty LastLapTimeProperty = DependencyProperty.Register("LastLapTime", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty PaceProperty = DependencyProperty.Register("Pace", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty BestLapProperty = DependencyProperty.Register("BestLap", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty CurrentLapProgressTimeProperty = DependencyProperty.Register("CurrentLapProgressTime", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty LastPitInfoProperty = DependencyProperty.Register("LastPitInfo", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty TimeToPlayerProperty = DependencyProperty.Register("TimeToPlayer", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty TopSpeedProperty = DependencyProperty.Register("TopSpeed", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector1Property = DependencyProperty.Register("Sector1", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector2Property = DependencyProperty.Register("Sector2", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });
        public static readonly DependencyProperty Sector3Property = DependencyProperty.Register("Sector3", typeof(ColumnSettingsModelView), typeof(ColumnsSettingsModelView), new PropertyMetadata { PropertyChangedCallback = PropertyChangedCallback });

        public event PropertyChangedEventHandler PropertyChanged;

        public ColumnSettingsModelView Position
        {
            get => (ColumnSettingsModelView)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public ColumnSettingsModelView Name
        {
            get => (ColumnSettingsModelView)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public ColumnSettingsModelView CarName
        {
            get => (ColumnSettingsModelView)GetValue(CarNameProperty);
            set => SetValue(CarNameProperty, value);
        }

        public ColumnSettingsModelView CompletedLaps
        {
            get => (ColumnSettingsModelView)GetValue(CompletedLapsProperty);
            set => SetValue(CompletedLapsProperty, value);
        }

        public ColumnSettingsModelView LastLapTime
        {
            get => (ColumnSettingsModelView)GetValue(LastLapTimeProperty);
            set => SetValue(LastLapTimeProperty, value);
        }

        public ColumnSettingsModelView Pace
        {
            get => (ColumnSettingsModelView)GetValue(PaceProperty);
            set => SetValue(PaceProperty, value);
        }

        public ColumnSettingsModelView BestLap
        {
            get => (ColumnSettingsModelView)GetValue(BestLapProperty);
            set => SetValue(BestLapProperty, value);
        }

        public ColumnSettingsModelView CurrentLapProgressTime
        {
            get => (ColumnSettingsModelView)GetValue(CurrentLapProgressTimeProperty);
            set => SetValue(CurrentLapProgressTimeProperty, value);
        }

        public ColumnSettingsModelView LastPitInfo
        {
            get => (ColumnSettingsModelView)GetValue(LastPitInfoProperty);
            set => SetValue(LastPitInfoProperty, value);
        }

        public ColumnSettingsModelView TimeToPlayer
        {
            get => (ColumnSettingsModelView)GetValue(TimeToPlayerProperty);
            set => SetValue(TimeToPlayerProperty, value);
        }

        public ColumnSettingsModelView TopSpeed
        {
            get => (ColumnSettingsModelView)GetValue(TopSpeedProperty);
            set => SetValue(TopSpeedProperty, value);
        }

        public ColumnSettingsModelView Sector1
        {
            get => (ColumnSettingsModelView)GetValue(Sector1Property);
            set => SetValue(Sector1Property, value);
        }

        public ColumnSettingsModelView Sector2
        {
            get => (ColumnSettingsModelView)GetValue(Sector2Property);
            set => SetValue(Sector2Property, value);
        }

        public ColumnSettingsModelView Sector3
        {
            get => (ColumnSettingsModelView)GetValue(Sector3Property);
            set => SetValue(Sector3Property, value);
        }

        public void FromModel(ColumnsSettings columnsSettings)
        {
            Position = ColumnSettingsModelView.CreateFromModel(columnsSettings.Position);
            Name = ColumnSettingsModelView.CreateFromModel(columnsSettings.Name);
            CarName = ColumnSettingsModelView.CreateFromModel(columnsSettings.CarName);
            CompletedLaps = ColumnSettingsModelView.CreateFromModel(columnsSettings.CompletedLaps);
            LastLapTime = ColumnSettingsModelView.CreateFromModel(columnsSettings.LastLapTime);
            Pace = ColumnSettingsModelView.CreateFromModel(columnsSettings.Pace);
            BestLap = ColumnSettingsModelView.CreateFromModel(columnsSettings.BestLap);
            CurrentLapProgressTime = ColumnSettingsModelView.CreateFromModel(columnsSettings.CurrentLapProgressTime);
            LastPitInfo = ColumnSettingsModelView.CreateFromModel(columnsSettings.LastPitInfo);
            TimeToPlayer = ColumnSettingsModelView.CreateFromModel(columnsSettings.TimeToPlayer);
            TopSpeed = ColumnSettingsModelView.CreateFromModel(columnsSettings.TopSpeed);
            Sector1 = ColumnSettingsModelView.CreateFromModel(columnsSettings.Sector1);
            Sector2 = ColumnSettingsModelView.CreateFromModel(columnsSettings.Sector2);
            Sector3 = ColumnSettingsModelView.CreateFromModel(columnsSettings.Sector3);
        }

        public static ColumnsSettingsModelView CreateFromModel(ColumnsSettings columnsSettings)
        {
            ColumnsSettingsModelView newColumnSettingsModelView = new ColumnsSettingsModelView();
            newColumnSettingsModelView.FromModel(columnsSettings);
            return newColumnSettingsModelView;
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
            if (d is ColumnsSettingsModelView columnsSettingsModelView)
            {
                columnsSettingsModelView.OnPropertyChanged(e.Property.Name);
            }
        }
    }
}