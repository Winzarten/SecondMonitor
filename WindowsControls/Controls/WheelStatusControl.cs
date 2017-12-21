using System.ComponentModel;
using System.Windows.Forms;
using SecondMonitor.DataModel;
using System.Drawing;

namespace SecondMonitor.CarStatus.Forms.Controls
{
    public partial class WheelStatusControl : UserControl
    {

        private static readonly int MaxGreen = 155;
        private static readonly int MaxRed = 255;
        private static readonly int MaxBlue = 155;

        public enum WheelPostionEnum { FrontLeft, FrontRight, RearLeft, RearRight };

        [Description("PositionOfWheel"),
        Category("Behaviour"),        
        Browsable(true)]
        public WheelPostionEnum WheelPostion
        {
            get;
            set;
        }

        [Description("DisplayTemperatureUnit"),
        Category("Behaviour"),
        DefaultValue(typeof(TemperatureUnits), "Celsius")
         ]
        public TemperatureUnits TemperatureDisplayUnit
        {
            get;
            set;
        }

        [Description("DisplayPressureUnit"),
        Category("Behaviour"),
        DefaultValue(typeof(TemperatureUnits), "Kpa")
         ]
        public PressureUnits PressureDisplayUnits
        {
            get;
            set;
        }

        private void UpdateTyreWearControl(SimulatorDataSet data)
        {
            double wear = 0;
            if (data.PlayerInfo == null)
                return;
            WheelInfo wheel = GetWheelByPosition(data);
            if (wheel == null)
                return;
            wear = wheel.TyreWear;
            lblTyreType.Text = wheel.TyreType;
            lblTyreType.Visible = wheel.TyreTypeFilled;
            pnlWear.Width = (int)((1 - wear) * this.Width);
            lbWear.Text = ((1 - wear) * 100).ToString("0");
        }

        private void UpdateBrakeControl(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            WheelInfo wheel = GetWheelByPosition(data);            
            if (wheel == null)
                return;
            lblBreakTemp.Text = wheel.BrakeTemperature.GetValueInUnits(TemperatureDisplayUnit).ToString("0");
            lblBreakTemp.PixelOn = ComputeColor(wheel.BrakeTemperature.InCelsius, wheel.OptimalBrakeTemperature.InCelsius, wheel.OptimalBrakeWindow);
        }

        private WheelInfo GetWheelByPosition(SimulatorDataSet data)
        {
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    return data.PlayerInfo.CarInfo.WheelsInfo.FrontLeft;
                case WheelPostionEnum.FrontRight:
                    return data.PlayerInfo.CarInfo.WheelsInfo.FrontRight;                    
                case WheelPostionEnum.RearLeft:
                    return data.PlayerInfo.CarInfo.WheelsInfo.RearLeft;
                case WheelPostionEnum.RearRight:
                    return data.PlayerInfo.CarInfo.WheelsInfo.RearRight;                    
            }
            return null;
        }

        private void UpdatePressureControl(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            WheelInfo wheel = GetWheelByPosition(data);
            if (wheel == null)
                return;
            lblTyrePressure.Text = wheel.TyrePressure.GetValueInUnits(PressureDisplayUnits).ToString("0");            
        }

        private Color ComputeColor(double value, double optimalValue, double window)
        {
            double threshold = window / 2;
            int r =0, g =0 , b =0;
            r = 0 + (int)((MaxRed - 0) * (value-optimalValue - threshold) / (threshold));
            if (value > optimalValue + threshold)
                g = MaxGreen + (int)((0 - MaxGreen) * (value - optimalValue - window) / (window));
            else
                g = 0 + (int)((MaxGreen - 0) * (value - optimalValue + window) / (window));

            b = MaxBlue + (int)((0 - MaxBlue) * (value - optimalValue + window) / (threshold));
            if (r < 0)
                r = 0;
            if (r > MaxRed)
                r = MaxRed;
            if (g < 0)
                g = 0;
            if (g > MaxGreen)
                g = MaxGreen;
            if (b < 0)
                b = 0;
            if (b > MaxBlue)
                b = MaxBlue;
            
                
            return Color.FromArgb(r, g, b);
        }

        private void UpdateWheelTemp(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            WheelInfo wheel = GetWheelByPosition(data);
            if (wheel == null)
                return;            
            wheelTempLeft.Text = wheel.LeftTyreTemp.GetValueInUnits(TemperatureDisplayUnit).ToString("0");
            wheelTempLeft.PixelOn = ComputeColor(wheel.LeftTyreTemp.InCelsius, wheel.OptimalTyreTemperature.InCelsius, wheel.OptimalTyreWindow);
            wheelTempCenter.Text = wheel.CenterTyreTemp.GetValueInUnits(TemperatureDisplayUnit).ToString("0");
            wheelTempCenter.PixelOn = ComputeColor(wheel.CenterTyreTemp.InCelsius, wheel.OptimalTyreTemperature.InCelsius, wheel.OptimalTyreWindow);
            wheelTempRight.Text = wheel.RightTyreTemp.GetValueInUnits(TemperatureDisplayUnit).ToString("0");
            wheelTempRight.PixelOn = ComputeColor(wheel.RightTyreTemp.InCelsius, wheel.OptimalTyreTemperature.InCelsius, wheel.OptimalTyreWindow);
        }

        public void UpdateControl(SimulatorDataSet data)
        {
            UpdateBrakeControl(data);
            UpdateWheelTemp(data);
            UpdatePressureControl(data);
            UpdateTyreWearControl(data);
        }

        public WheelStatusControl()
        {            
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.PerformAutoScale();
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {

        }

        private void WheelStatusControl_Load(object sender, System.EventArgs e)
        {

        }
    }
}
