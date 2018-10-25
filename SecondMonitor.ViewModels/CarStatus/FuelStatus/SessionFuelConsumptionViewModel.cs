﻿namespace SecondMonitor.ViewModels.CarStatus.FuelStatus
{
    using System.Windows;
    using DataModel.BasicProperties;

    public class SessionFuelConsumptionViewModel : AbstractViewModel<SessionFuelConsumptionInfo>
    {
        private static readonly DependencyProperty TrackNameProperty = DependencyProperty.Register("TrackName", typeof(string), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty SessionTypeProperty = DependencyProperty.Register("SessionType", typeof(string), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty FuelConsumptionProperty = DependencyProperty.Register("FuelConsumption", typeof(FuelConsumptionInfo), typeof(SessionFuelConsumptionViewModel));
        private static readonly DependencyProperty LapDistanceProperty = DependencyProperty.Register("LapDistance", typeof(Distance), typeof(SessionFuelConsumptionViewModel));

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

        public FuelConsumptionInfo FuelConsumption
        {
            get => (FuelConsumptionInfo) GetValue(FuelConsumptionProperty);
            set => SetValue(FuelConsumptionProperty, value);
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
    }
}