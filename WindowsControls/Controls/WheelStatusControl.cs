using System.ComponentModel;
using System.Windows.Forms;
using SecondMonitor.DataModel;

namespace SecondMonitor.CarStatus.Forms.Controls
{
    public partial class WheelStatusControl : UserControl
    {
        public enum WheelPostionEnum { FrontLeft, FrontRight, RearLeft, RearRight };

        [Description("PositionOfWheel"),
        Category("Behaviour"),        
        Browsable(true)]
        public WheelPostionEnum WheelPostion
        {
            get;
            set;
        }

        private void UpdateTyreWearControl(SimulatorDataSet data)
        {
            double wear = 0;
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    wear = data.PlayerCarInfo.WheelsInfo.FrontLeft.TyreWear;
                    lblTyreType.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.TyreType;
                    lblTyreType.Visible = data.PlayerCarInfo.WheelsInfo.FrontLeft.TyreTypeFilled;
                    break;
                case WheelPostionEnum.FrontRight:
                    wear = data.PlayerCarInfo.WheelsInfo.FrontRight.TyreWear;
                    lblTyreType.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.TyreType;
                    lblTyreType.Visible = data.PlayerCarInfo.WheelsInfo.FrontRight.TyreTypeFilled;
                    break;
                case WheelPostionEnum.RearLeft:
                    wear = data.PlayerCarInfo.WheelsInfo.RearLeft.TyreWear;
                    lblTyreType.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.TyreType;
                    lblTyreType.Visible = data.PlayerCarInfo.WheelsInfo.RearLeft.TyreTypeFilled;
                    break;
                case WheelPostionEnum.RearRight:
                    wear = data.PlayerCarInfo.WheelsInfo.RearRight.TyreWear;
                    lblTyreType.Text = data.PlayerCarInfo.WheelsInfo.RearRight.TyreType;
                    lblTyreType.Visible = data.PlayerCarInfo.WheelsInfo.RearRight.TyreTypeFilled;
                    break;

            }
            pnlWear.Width = (int)((1 - wear) * this.Width);
            lbWear.Text = ((1 - wear) * 100).ToString("0");
        }

        private void UpdateBrakeControl(SimulatorDataSet data)
        {
            switch(WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    lblBreakTemp.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.BrakeTemperature.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    lblBreakTemp.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.BrakeTemperature.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    lblBreakTemp.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.BrakeTemperature.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    lblBreakTemp.Text = data.PlayerCarInfo.WheelsInfo.RearRight.BrakeTemperature.InCelsius.ToString("0");
                    break;

            }
        }

        private void UpdatePressureControl(SimulatorDataSet data)
        {            
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    lblTyrePressure.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.TyrePressure.InKpa.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    lblTyrePressure.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.TyrePressure.InKpa.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    lblTyrePressure.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.TyrePressure.InKpa.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    lblTyrePressure.Text = data.PlayerCarInfo.WheelsInfo.RearRight.TyrePressure.InKpa.ToString("0");
                    break;

            }
        }

        private void UpdateWheelTemp(SimulatorDataSet data)
        {
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    wheelTempLeft.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.LeftTyreTemp.InCelsius.ToString("0");
                    wheelTempCenter.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.CenterTyreTemp.InCelsius.ToString("0");
                    wheelTempRight.Text = data.PlayerCarInfo.WheelsInfo.FrontLeft.RightTyreTemp.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    wheelTempLeft.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.LeftTyreTemp.InCelsius.ToString("0");
                    wheelTempCenter.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.CenterTyreTemp.InCelsius.ToString("0");
                    wheelTempRight.Text = data.PlayerCarInfo.WheelsInfo.FrontRight.RightTyreTemp.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    wheelTempLeft.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.LeftTyreTemp.InCelsius.ToString("0");
                    wheelTempCenter.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.CenterTyreTemp.InCelsius.ToString("0");
                    wheelTempRight.Text = data.PlayerCarInfo.WheelsInfo.RearLeft.RightTyreTemp.InCelsius.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    wheelTempLeft.Text = data.PlayerCarInfo.WheelsInfo.RearRight.LeftTyreTemp.InCelsius.ToString("0");
                    wheelTempCenter.Text = data.PlayerCarInfo.WheelsInfo.RearRight.CenterTyreTemp.InCelsius.ToString("0");
                    wheelTempRight.Text = data.PlayerCarInfo.WheelsInfo.RearRight.RightTyreTemp.InCelsius.ToString("0");
                    break;

            }
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
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {

        }
    }
}
