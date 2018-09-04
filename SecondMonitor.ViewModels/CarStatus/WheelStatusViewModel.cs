namespace SecondMonitor.ViewModels.CarStatus
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot.Systems;
    using SecondMonitor.ViewModels.Annotations;

    public class WheelStatusViewModel : DependencyObject, IWheelStatusViewModel, INotifyPropertyChanged
    {
        private static readonly DependencyProperty TemperatureUnitsProperty = DependencyProperty.Register("TemperatureUnits", typeof(TemperatureUnits), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty PressureUnitsProperty = DependencyProperty.Register("PressureUnits", typeof(PressureUnits), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreConditionProperty = DependencyProperty.Register("TyreCondition", typeof(double), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreCoreTemperatureProperty = DependencyProperty.Register("TyreCoreTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreLeftTemperatureProperty = DependencyProperty.Register("TyreLeftTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreCenterTemperatureProperty = DependencyProperty.Register("TyreCenterTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreRightTemperatureProperty = DependencyProperty.Register("TyreRightTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty BrakeTemperatureProperty = DependencyProperty.Register("BrakeTemperature", typeof(OptimalQuantity<Temperature>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyreSlippingIndicationProperty = DependencyProperty.Register("TyreSlippingIndication", typeof(bool), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty TyrePressureProperty = DependencyProperty.Register("TyrePressure", typeof(OptimalQuantity<Pressure>), typeof(WheelStatusViewModel));
        private static readonly DependencyProperty IsLeftWheelProperty = DependencyProperty.Register("IsLeftWheel", typeof(bool), typeof(WheelStatusViewModel));

        public WheelStatusViewModel(bool isLeft)
        {
            IsLeftWheel = isLeft;
        }

        public bool TyreSlippingIndication
        {
            get => (bool)GetValue(TyreSlippingIndicationProperty);
            set => SetValue(TyreSlippingIndicationProperty, value);
        }

        public TemperatureUnits TemperatureUnits
        {
            get => (TemperatureUnits)GetValue(TemperatureUnitsProperty);
            set => SetValue(TemperatureUnitsProperty, value);
        }

        public PressureUnits PressureUnits
        {
            get => (PressureUnits)GetValue(PressureUnitsProperty);
            set => SetValue(PressureUnitsProperty, value);
        }

        public double TyreCondition
        {
            get => (double)GetValue(TyreConditionProperty);
            set => SetValue(TyreConditionProperty, value);
        }

        public OptimalQuantity<Temperature> TyreCoreTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreCoreTemperatureProperty);
            set => SetValue(TyreCoreTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreLeftTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreLeftTemperatureProperty);
            set => SetValue(TyreLeftTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreCenterTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreCenterTemperatureProperty);
            set => SetValue(TyreCenterTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> TyreRightTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(TyreRightTemperatureProperty);
            set => SetValue(TyreRightTemperatureProperty, value);
        }

        public OptimalQuantity<Temperature> BrakeTemperature
        {
            get => (OptimalQuantity<Temperature>)GetValue(BrakeTemperatureProperty);
            set => SetValue(BrakeTemperatureProperty, value);
        }

        public OptimalQuantity<Pressure> TyrePressure
        {
            get => (OptimalQuantity<Pressure>)GetValue(TyrePressureProperty);
            set => SetValue(TyrePressureProperty, value);
        }

        public bool IsLeftWheel
        {
            get => (bool)GetValue(IsLeftWheelProperty);
            set => SetValue(IsLeftWheelProperty, value);
        }

        public void ApplyWheelCondition(WheelInfo wheelInfo)
        {
            TyreCondition = 100 * (1 -wheelInfo.TyreWear);

            if (wheelInfo.TyreCoreTemperature.ActualQuantity.InCelsius > -200 && (TyreCoreTemperature == null || Math.Abs(TyreCoreTemperature.ActualQuantity.RawValue - wheelInfo.TyreCoreTemperature.ActualQuantity.RawValue) > 0.5))
            {
                TyreCoreTemperature = wheelInfo.TyreCoreTemperature;
            }

            if (TyreLeftTemperature == null || Math.Abs(TyreLeftTemperature.ActualQuantity.RawValue - wheelInfo.LeftTyreTemp.ActualQuantity.RawValue) > 0.5)
            {
                TyreLeftTemperature = wheelInfo.LeftTyreTemp;
            }

            if (wheelInfo.CenterTyreTemp.ActualQuantity.InCelsius > -200 && (TyreCenterTemperature == null || Math.Abs(TyreCenterTemperature.ActualQuantity.RawValue - wheelInfo.CenterTyreTemp.ActualQuantity.RawValue) > 0.5))
            {
                TyreCenterTemperature = wheelInfo.CenterTyreTemp;
            }

            if (TyreRightTemperature == null || Math.Abs(TyreRightTemperature.ActualQuantity.RawValue - wheelInfo.RightTyreTemp.ActualQuantity.RawValue) > 0.5)
            {

                TyreRightTemperature = wheelInfo.RightTyreTemp;
            }

            if (BrakeTemperature == null || Math.Abs(BrakeTemperature.ActualQuantity.RawValue - wheelInfo.BrakeTemperature.ActualQuantity.RawValue) > 0.5)
            {
                BrakeTemperature = wheelInfo.BrakeTemperature;
            }

            TyrePressure = wheelInfo.TyrePressure;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}