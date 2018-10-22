namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;

    using BasicProperties;

    [Serializable]
    public class WheelInfo
    {
        private static readonly Temperature OptimalTemperature = Temperature.FromCelsius(85);
        private static readonly Temperature OptimalTemperatureWindow = Temperature.FromCelsius(10);

        public WheelInfo()
        {
            BrakeTemperature = new OptimalQuantity<Temperature>()
                                   {
                                       IdealQuantity = Temperature.FromCelsius(350),
                                       IdealQuantityWindow = Temperature.FromCelsius(200),
                                       ActualQuantity = Temperature.Zero
                                   };
            TyrePressure = new OptimalQuantity<Pressure>()
                               {
                                   IdealQuantity = Pressure.Zero,
                                   IdealQuantityWindow = Pressure.Zero,
                                   ActualQuantity = Pressure.Zero
                               };
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

            TyreCoreTemperature = new OptimalQuantity<Temperature>()
                                 {
                                     IdealQuantity = OptimalTemperature,
                                     IdealQuantityWindow = OptimalTemperatureWindow
                                 };

            TyreWear = new TyreWear(){ActualWear = 0.0, NoWearWearLimit = 0.10, LightWearLimit = 0.25, HeavyWearLimit = 0.7};
            TyreType = "Base";
        }

        public OptimalQuantity<Temperature> BrakeTemperature { get; set; }

        public OptimalQuantity<Pressure> TyrePressure { get; set; }


        public string TyreType { get; set; }

        public TyreWear TyreWear { get; set; }

        public OptimalQuantity<Temperature> LeftTyreTemp { get; set; }

        public OptimalQuantity<Temperature> RightTyreTemp{ get; set; }

        public OptimalQuantity<Temperature> CenterTyreTemp { get; set; }

        public OptimalQuantity<Temperature> TyreCoreTemperature { get; set; }

    }

}
