namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System.Windows;
    using Contracts.FuelInformation;
    using DataModel.BasicProperties;

    public class SessionFuelConsumptionViewModel : AbstractViewModel<SessionFuelConsumptionInfo>, ISessionFuelConsumptionViewModel
    {
        private static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register("TrackName", typeof(string), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty SessionTypeProperty = DependencyProperty.Register("SessionType", typeof(string), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty FuelConsumptionProperty = DependencyProperty.Register("FuelConsumption", typeof(IFuelConsumptionInfo), typeof(SessionFuelConsumptionViewModel), new FrameworkPropertyMetadata(){ PropertyChangedCallback = OnFuelConsumptionChanged});
        private static readonly DependencyProperty LapDistanceProperty = DependencyProperty.Register("LapDistance", typeof(Distance), typeof(SessionFuelConsumptionViewModel), new FrameworkPropertyMetadata() { PropertyChangedCallback = OnFuelConsumptionChanged });
        private static readonly DependencyProperty AvgPerMinuteProperty = DependencyProperty.Register("AvgPerMinute", typeof(Volume), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty AvgPerLapProperty = DependencyProperty.Register("AvgPerLap", typeof(Volume), typeof(SessionFuelConsumptionViewModel));

        public string TrackName
        {
            get => (string) GetValue(TrackNameProperty);
            set => SetValue(TrackNameProperty, value);
        }

        public Distance LapDistance
        {
            get => (Distance) GetValue(LapDistanceProperty);
            set => SetValue(LapDistanceProperty, value);
        }

        public string SessionType
        {
            get => (string) GetValue(SessionTypeProperty);
            set => SetValue(SessionTypeProperty, value);
        }

        public IFuelConsumptionInfo FuelConsumption
        {
            get => (IFuelConsumptionInfo) GetValue(FuelConsumptionProperty);
            set => SetValue(FuelConsumptionProperty, value);
        }

        public Volume AvgPerMinute
        {
            get => (Volume)GetValue(AvgPerMinuteProperty);
            set => SetValue(AvgPerMinuteProperty, value);
        }

        public Volume AvgPerLap
        {
            get => (Volume)GetValue(AvgPerLapProperty);
            set => SetValue(AvgPerLapProperty, value);
        }

        public override void FromModel(SessionFuelConsumptionInfo model)
        {
            TrackName = model.TrackName;
            LapDistance = model.LapDistance;
            FuelConsumption = model.FuelConsumptionInfo;
            SessionType = model.SessionType.ToString();
        }

        public override SessionFuelConsumptionInfo SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }

        private void OnFuelConsumptionChanged()
        {
            if (FuelConsumption == null || LapDistance == null)
            {
                return;
            }

            AvgPerMinute = FuelConsumption.GetAveragePerMinute();
            AvgPerLap = FuelConsumption.GetAveragePerDistance(LapDistance);
        }

        private static void OnFuelConsumptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SessionFuelConsumptionViewModel viewModel)
            {
                viewModel.OnFuelConsumptionChanged();
            }
        }
    }
}