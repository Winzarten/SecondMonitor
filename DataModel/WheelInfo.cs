﻿namespace SecondMonitor.DataModel
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
        }
        public Temperature BrakeTemperature;
        public Pressure TyrePressure;
        public Temperature LeftTyreTemp;
        public Temperature RightTyreTemp;
        public Temperature CenterTyreTemp;
    }

}
