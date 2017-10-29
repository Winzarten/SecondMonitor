namespace SecondMonitor.DataModel
{
    public class WheelInfo
    {                
        public WheelInfo()
        {
            BrakeTemperature = new Temperature();
            TyrePressure = new Pressure();
            LeftTyreTemp = new Temperature();
            RightTyreTemp = new Temperature();
            CenterTyreTemp = new Temperature();
            TyreWear = 0;
            TyreTypeFilled = false;
        }
        public Temperature BrakeTemperature;
        public Pressure TyrePressure;
        public Temperature LeftTyreTemp;
        public Temperature RightTyreTemp;
        public Temperature CenterTyreTemp;
        public string TyreType;
        public bool TyreTypeFilled;
        public double TyreWear;

        public Temperature OptimalTyreTemperature = Temperature.FromCelsius(85);
        public double OptimpalTyreWindow = 15;

        public Temperature OptimalBrakeTemperature = Temperature.FromCelsius(350);
        public double OptimpalBrakeWindow = 200;

    }

}
