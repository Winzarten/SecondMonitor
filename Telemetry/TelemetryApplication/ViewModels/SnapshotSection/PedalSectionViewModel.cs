namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SnapshotSection
{
    using DataModel.BasicProperties;
    using DataModel.Telemetry;

    public class PedalSectionViewModel : SnapshotViewModel, IPedalSectionViewModel
    {
        private double _throttlePosition;
        private double _brakePosition;
        private double _clutchPosition;
        private string _gear;
        private double _steerAngle;
        private int _rpm;
        private Velocity _speed;
        private VelocityUnits _velocityUnits;


        public double ThrottlePosition
        {
            get => _throttlePosition;
            set
            {
                _throttlePosition = value;
                NotifyPropertyChanged();
            }
        }

        public double BrakePosition
        {
            get => _brakePosition;
            set
            {
                _brakePosition = value;
                NotifyPropertyChanged();
            }
        }

        public double ClutchPosition
        {
            get => _clutchPosition;
            set
            {
                _clutchPosition = value;
                NotifyPropertyChanged();
            }
        }

        public string Gear
        {
            get => _gear;
            set
            {
                _gear = value;
                NotifyPropertyChanged();
            }
        }

        public double SteerAngle
        {
            get => _steerAngle;
            set
            {
                _steerAngle = value;
                NotifyPropertyChanged();
            }
        }

        public int Rpm
        {
            get => _rpm;
            set
            {
                _rpm = value;
                NotifyPropertyChanged();
            }
        }

        public Velocity Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NotifyPropertyChanged();
            }
        }

        public VelocityUnits VelocityUnits
        {
            get => _velocityUnits;
            set
            {
                _velocityUnits = value;
                NotifyPropertyChanged();
            }
        }


        protected override void ApplyModel(TimedTelemetrySnapshot model)
        {
            ThrottlePosition = model.InputInfo.ThrottlePedalPosition * 100.0;
            BrakePosition = model.InputInfo.BrakePedalPosition * 100.0;
            ClutchPosition = model.InputInfo.ClutchPedalPosition * 100.0;
            SteerAngle = model.InputInfo.WheelAngle;
            Gear = model.PlayerData.CarInfo.CurrentGear;
            Rpm = model.PlayerData.CarInfo.EngineRpm;
            Speed = model.PlayerData.Speed;
        }
    }
}