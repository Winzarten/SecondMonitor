namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using SecondMonitor.DataModel.BasicProperties;

    public class WheelInfo
    {                
        public WheelInfo()
        {
            this.BrakeTemperature = new Temperature();
            this.TyrePressure = new Pressure();
            this.LeftTyreTemp = new Temperature();
            this.RightTyreTemp = new Temperature();
            this.CenterTyreTemp = new Temperature();
            this.TyreWear = 0;
            this.TyreTypeFilled = false;
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
