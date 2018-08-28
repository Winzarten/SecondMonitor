namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using BasicProperties;

    public class WheelInfo
    {
        private static readonly Temperature OptimalTemperature = Temperature.FromCelsius(85);
        private static readonly Temperature OptimalTemperatureWindow = Temperature.FromCelsius(15);

        public WheelInfo()
        {
            BrakeTemperature = Temperature.Zero;
            TyrePressure = new Pressure();
            LeftTyreTemp = new OptimalQuantity<Temperature>()
                               {
                                   IdealQuantity = OptimalTemperature,
                                   IdealQuantityWindow = OptimalTemperatureWindow
                               };
            RightTyreTemp = new OptimalQuantity<Temperature>()
                                 {
                                     IdealQuantity = OptimalTemperature,
                                     IdealQuantityWindow = OptimalTemperatureWindow
                                 };
            CenterTyreTemp = new OptimalQuantity<Temperature>()
                                 {
                                     IdealQuantity = OptimalTemperature,
                                     IdealQuantityWindow = OptimalTemperatureWindow
                                 };
            TyreWear = 0;
            TyreTypeFilled = false;
        }

        public Temperature BrakeTemperature { get; set; }

        public Pressure TyrePressure { get; set; }


        public string TyreType { get; set; }

        public bool TyreTypeFilled { get; set; }

        public double TyreWear { get; set; }

        public OptimalQuantity<Temperature> LeftTyreTemp { get; set; }

        public OptimalQuantity<Temperature> RightTyreTemp{ get; set; }

        public OptimalQuantity<Temperature> CenterTyreTemp { get; set; }

        public Temperature OptimalBrakeTemperature { get; set; } = Temperature.FromCelsius(350);

        public double OptimalBrakeWindow { get; set; } = 200;

    }

}
