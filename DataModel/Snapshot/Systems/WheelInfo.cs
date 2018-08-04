namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using BasicProperties;

    public class WheelInfo
    {
        public WheelInfo()
        {
            BrakeTemperature = Temperature.Zero;
            TyrePressure = new Pressure();
            LeftTyreTemp = Temperature.Zero;
            RightTyreTemp = Temperature.Zero;
            CenterTyreTemp = Temperature.Zero;
            TyreWear = 0;
            TyreTypeFilled = false;
        }

        public Temperature BrakeTemperature { get; set; }

        public Pressure TyrePressure { get; set; }

        public Temperature LeftTyreTemp { get; set; }

        public Temperature RightTyreTemp { get; set; }

        public Temperature CenterTyreTemp { get; set; }

        public string TyreType { get; set; }

        public bool TyreTypeFilled { get; set; }

        public double TyreWear { get; set; }

        public Temperature OptimalTyreTemperature { get; set; } = Temperature.FromCelsius(85);

        public double OptimalTyreWindow { get; set; } = 15;

        public Temperature OptimalBrakeTemperature { get; set; } = Temperature.FromCelsius(350);

        public double OptimalBrakeWindow { get; set; } = 200;

    }

}
