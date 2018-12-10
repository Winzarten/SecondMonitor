namespace SecondMonitor.WindowsControls.WPF.FuelControl
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using DataModel.BasicProperties;

    public class FuelOverviewControl : Control, INotifyPropertyChanged
    {
        private static readonly DependencyProperty FuelPercentageProperty = DependencyProperty.Register("FuelPercentage", typeof(double), typeof(FuelOverviewControl), new PropertyMetadata(){ PropertyChangedCallback = OnFuelLevelChanged});
        private static readonly DependencyProperty BlinkToColorProperty = DependencyProperty.Register("BlinkToColor", typeof(Color), typeof(FuelOverviewControl), new FrameworkPropertyMetadata {PropertyChangedCallback = OnColorChanged});
        private static readonly DependencyProperty FuelStatusColorProperty = DependencyProperty.Register("FuelStatusColor", typeof(SolidColorBrush), typeof(FuelOverviewControl));
        private static readonly DependencyProperty FuelStatusUnknownColorProperty = DependencyProperty.Register("FuelStatusUnknownColor", typeof(Color), typeof(FuelOverviewControl), new FrameworkPropertyMetadata { PropertyChangedCallback = OnColorChanged });
        private static readonly DependencyProperty FuelStatusOkColorProperty = DependencyProperty.Register("FuelStatusOkColor", typeof(Color), typeof(FuelOverviewControl), new FrameworkPropertyMetadata { PropertyChangedCallback = OnColorChanged });
        private static readonly DependencyProperty FuelStatusNotEnoughColorProperty = DependencyProperty.Register("FuelStatusNotEnoughColor", typeof(Color), typeof(FuelOverviewControl), new FrameworkPropertyMetadata { PropertyChangedCallback = OnColorChanged });
        private static readonly DependencyProperty FuelStatusMightBeColorProperty = DependencyProperty.Register("FuelStatusMightBeColor", typeof(Color), typeof(FuelOverviewControl), new FrameworkPropertyMetadata { PropertyChangedCallback = OnColorChanged });
        private static readonly DependencyProperty FuelStateProperty = DependencyProperty.Register("FuelState", typeof(FuelLevelStatus), typeof(FuelOverviewControl), new PropertyMetadata {PropertyChangedCallback = OnFuelStateChanged});
        private static readonly DependencyProperty MaximumFuelProperty = DependencyProperty.Register("MaximumFuel", typeof(Volume), typeof(FuelOverviewControl), new PropertyMetadata() { PropertyChangedCallback = OnFuelLevelChanged});
        private static readonly DependencyProperty TimeLeftProperty  = DependencyProperty.Register("TimeLeft", typeof(TimeSpan), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty LapsLeftProperty = DependencyProperty.Register("LapsLeft", typeof(double), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty AvgPerLapProperty = DependencyProperty.Register("AvgPerLap", typeof(Volume), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty AvgPerMinuteProperty = DependencyProperty.Register("AvgPerMinute", typeof(Volume), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty CurPerLapProperty = DependencyProperty.Register("CurPerLap", typeof(Volume), typeof(FuelOverviewControl));
        private static readonly DependencyProperty CurPerMinuteProperty = DependencyProperty.Register("CurPerMinute", typeof(Volume), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty ResetCommandProperty = DependencyProperty.Register("ResetCommand", typeof(ICommand), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty VolumeUnitsProperty = DependencyProperty.Register("VolumeUnits", typeof(VolumeUnits), typeof(FuelOverviewControl), new PropertyMetadata());
        private static readonly DependencyProperty FuelCalculatorCommandProperty = DependencyProperty.Register("FuelCalculatorCommand", typeof(ICommand), typeof(FuelOverviewControl), new PropertyMetadata());
        public static readonly DependencyProperty ShowAdditionalInfoProperty = DependencyProperty.Register("ShowAdditionalInfo", typeof(bool), typeof(FuelOverviewControl));
        public static readonly DependencyProperty FuelDeltaProperty = DependencyProperty.Register("FuelDelta", typeof(Volume), typeof(FuelOverviewControl));
        public static readonly DependencyProperty LapsDeltaProperty = DependencyProperty.Register("LapsDelta", typeof(double), typeof(FuelOverviewControl));
        public static readonly DependencyProperty TimeDeltaProperty = DependencyProperty.Register("TimeDelta", typeof(TimeSpan), typeof(FuelOverviewControl));

        public TimeSpan TimeDelta
        {
            get => (TimeSpan) GetValue(TimeDeltaProperty);
            set => SetValue(TimeDeltaProperty, value);
        }

        public double LapsDelta
        {
            get => (double) GetValue(LapsDeltaProperty);
            set => SetValue(LapsDeltaProperty, value);
        }

        public Volume FuelDelta
        {
            get => (Volume) GetValue(FuelDeltaProperty);
            set => SetValue(FuelDeltaProperty, value);
        }

        public bool ShowAdditionalInfo
        {
            get => (bool) GetValue(ShowAdditionalInfoProperty);
            set => SetValue(ShowAdditionalInfoProperty, value);
        }




        private Storyboard _toOkStoryBoard;
        private Storyboard _toUnknownStoryBoard;
        private Storyboard _toPossibleEnoughStoryBoard;
        private Storyboard _toNotEnoughStoryBoard;
        private Storyboard _criticalStoryBoard;

        private bool _criticalStoryBoardStarted;

        public ICommand ResetCommand
        {
            get => (ICommand)GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        public ICommand FuelCalculatorCommand
        {
            get => (ICommand)GetValue(FuelCalculatorCommandProperty);
            set => SetValue(FuelCalculatorCommandProperty, value);
        }

        public TimeSpan TimeLeft
        {
            get => (TimeSpan)GetValue(TimeLeftProperty);
            set => SetValue(TimeLeftProperty, value);
        }

        public VolumeUnits VolumeUnits
        {
            get => (VolumeUnits)GetValue(VolumeUnitsProperty);
            set => SetValue(VolumeUnitsProperty, value);
        }

        public double LapsLeft
        {
            get => (double)GetValue(LapsLeftProperty);
            set => SetValue(LapsLeftProperty, value);
        }

        public Volume AvgPerLap
        {
            get => (Volume)GetValue(AvgPerLapProperty);
            set => SetValue(AvgPerLapProperty, value);
        }

        public Volume AvgPerMinute
        {
            get => (Volume)GetValue(AvgPerMinuteProperty);
            set => SetValue(AvgPerMinuteProperty , value);
        }

        public Volume CurPerLap
        {
            get => (Volume)GetValue(CurPerLapProperty);
            set => SetValue(CurPerLapProperty, value);
        }

        public Volume CurPerMinute
        {
            get => (Volume)GetValue(CurPerMinuteProperty);
            set => SetValue(CurPerMinuteProperty, value);
        }

        public double FuelPercentage
        {
            get => (double)GetValue(FuelPercentageProperty);
            set => SetValue(FuelPercentageProperty, value);
        }

        public SolidColorBrush FuelStatusColor
        {
            get => (SolidColorBrush)GetValue(FuelStatusColorProperty);
            set => SetValue(FuelStatusColorProperty, value);
        }

        public Color FuelStatusUnknownColor
        {
            get => (Color)GetValue(FuelStatusUnknownColorProperty);
            set => SetValue(FuelStatusUnknownColorProperty, value);
        }

        public FuelLevelStatus FuelState
        {
            get => (FuelLevelStatus)GetValue(FuelStateProperty);
            set => SetValue(FuelStateProperty, value);
        }

        public Color FuelStatusOkColor
        {
            get => (Color)GetValue(FuelStatusOkColorProperty);
            set => SetValue(FuelStatusOkColorProperty, value);
        }


        public Color BlinkToColor
        {
            get => (Color)GetValue(BlinkToColorProperty);
            set => SetValue(BlinkToColorProperty, value);
        }

        public Color FuelStatusNotEnoughColor
        {
            get => (Color)GetValue(FuelStatusNotEnoughColorProperty);
            set => SetValue(FuelStatusNotEnoughColorProperty, value);
        }

        public Color FuelStatusMightBeColor
        {
            get => (Color)GetValue(FuelStatusMightBeColorProperty);
            set => SetValue(FuelStatusMightBeColorProperty, value);
        }

        public Volume MaximumFuel
        {
            get => (Volume)GetValue(MaximumFuelProperty);
            set => SetValue(MaximumFuelProperty,value);
        }

        public Volume FuelLeft
        {
            get;
            private set;
        }

        private void StartProperStoryBoard()
        {
            if (_toOkStoryBoard == null || _toNotEnoughStoryBoard == null || _toPossibleEnoughStoryBoard == null
                || _toNotEnoughStoryBoard == null || _criticalStoryBoard == null)
            {
                return;
            }

            try
            {
                if (_criticalStoryBoardStarted && _criticalStoryBoard.GetCurrentState(this) == ClockState.Active)
                {
                    _criticalStoryBoard.Stop(this);
                    _criticalStoryBoardStarted = false;
                }
            }
            catch (InvalidOperationException)
            {

            }

            switch (FuelState)
            {
                case FuelLevelStatus.Unknown:
                    _toUnknownStoryBoard.Begin(this);
                    break;
                case FuelLevelStatus.IsEnoughForSession:
                    _toOkStoryBoard.Begin(this);
                    break;
                case FuelLevelStatus.PossiblyEnoughForSession:
                    _toPossibleEnoughStoryBoard.Begin(this);
                    break;
                case FuelLevelStatus.NotEnoughForSession:
                    _toNotEnoughStoryBoard.Begin(this);
                    break;
                case FuelLevelStatus.Critical:
                    _criticalStoryBoard.Begin(this);
                    _criticalStoryBoardStarted = true;
                    break;
            }
        }

        private void UpdateStoryBoards()
        {
            _toOkStoryBoard = PrepareStoryBoard(FuelStatusOkColor);
            _toUnknownStoryBoard = PrepareStoryBoard(FuelStatusUnknownColor);
            _toPossibleEnoughStoryBoard = PrepareStoryBoard(FuelStatusMightBeColor);
            _toNotEnoughStoryBoard = PrepareStoryBoard(FuelStatusNotEnoughColor);
            _criticalStoryBoard = PrepareCriticalStoryBoard();
        }


        private Storyboard PrepareStoryBoard(Color endColor)
        {
            ColorAnimation toColorAnimation = new ColorAnimation(endColor, TimeSpan.FromSeconds(1));
            Storyboard sb = new Storyboard();
            sb.Children.Add(toColorAnimation);
            Storyboard.SetTarget(toColorAnimation, this);
            Storyboard.SetTargetProperty(toColorAnimation, new PropertyPath("FuelStatusColor.Color"));
            return sb;
        }

        private Storyboard PrepareCriticalStoryBoard()
        {
            ColorAnimation toRedAnimation = new ColorAnimation(FuelStatusNotEnoughColor, BlinkToColor, TimeSpan.FromSeconds(1));
            ColorAnimation backAnimation = new ColorAnimation(BlinkToColor, FuelStatusNotEnoughColor, TimeSpan.FromSeconds(1));
            backAnimation.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard sb = new Storyboard();
            sb.Children.Add(toRedAnimation);
            sb.Children.Add(backAnimation);
            sb.Duration = TimeSpan.FromSeconds(2);
            Storyboard.SetTarget(toRedAnimation, this);
            Storyboard.SetTargetProperty(toRedAnimation, new PropertyPath("FuelStatusColor.Color"));
            Storyboard.SetTarget(backAnimation, this);
            Storyboard.SetTargetProperty(backAnimation, new PropertyPath("FuelStatusColor.Color"));
            sb.RepeatBehavior = RepeatBehavior.Forever;
            return sb;
        }

        private void UpdateFuelLevels()
        {
            if (MaximumFuel == null)
            {
                return;
            }

            FuelLeft = MaximumFuel * FuelPercentage / 100;
            NotifyPropertyChanged(nameof(FuelLeft));
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FuelOverviewControl fuelOverviewControl)
            {
                fuelOverviewControl.UpdateStoryBoards();
            }
        }

        private static void OnFuelStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FuelOverviewControl fuelOverviewControl)
            {
                fuelOverviewControl.StartProperStoryBoard();
            }
        }

        private static void OnFuelLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FuelOverviewControl fuelOverviewControl)
            {
                fuelOverviewControl.UpdateFuelLevels();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}