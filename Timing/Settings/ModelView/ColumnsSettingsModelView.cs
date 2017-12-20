namespace SecondMonitor.Timing.Settings.ModelView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.Timing.Annotations;
    using SecondMonitor.Timing.Settings.Model;

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

        public void FromModel(ColumnsSettings columnsSettings)
        {
            this.Position = ColumnSettingsModelView.CreateFromModel(columnsSettings.Position);
            this.Name = ColumnSettingsModelView.CreateFromModel(columnsSettings.Name);
            this.CarName = ColumnSettingsModelView.CreateFromModel(columnsSettings.CarName);
            this.CompletedLaps = ColumnSettingsModelView.CreateFromModel(columnsSettings.CompletedLaps);
            this.LastLapTime = ColumnSettingsModelView.CreateFromModel(columnsSettings.LastLapTime);
            this.Pace = ColumnSettingsModelView.CreateFromModel(columnsSettings.Pace);
            this.BestLap = ColumnSettingsModelView.CreateFromModel(columnsSettings.BestLap);
            this.CurrentLapProgressTime = ColumnSettingsModelView.CreateFromModel(columnsSettings.CurrentLapProgressTime);
            this.LastPitInfo = ColumnSettingsModelView.CreateFromModel(columnsSettings.LastPitInfo);
            this.TimeToPlayer = ColumnSettingsModelView.CreateFromModel(columnsSettings.TimeToPlayer);
            this.TopSpeed = ColumnSettingsModelView.CreateFromModel(columnsSettings.TopSpeed);
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
                           Position = this.Position.ToModel(),
                           Name = this.Name.ToModel(),
                           CarName = this.CarName.ToModel(),
                           CompletedLaps = this.CompletedLaps.ToModel(),
                           LastLapTime = this.LastLapTime.ToModel(),
                           Pace = this.Pace.ToModel(),
                           BestLap = this.BestLap.ToModel(),
                           CurrentLapProgressTime = this.CurrentLapProgressTime.ToModel(),
                           LastPitInfo = this.LastPitInfo.ToModel(),
                           TimeToPlayer = this.TimeToPlayer.ToModel(),
                           TopSpeed = this.TopSpeed.ToModel(),
            };

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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