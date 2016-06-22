using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondWindow.Core.R3EConnector.Data;

namespace SecondWindow.CarStatus.Forms.Controls
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

        private void UpdateBrakeControl(R3ESharedData data)
        {
            switch(WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    lblBreakTemp.Text = data.BrakeTemp.FrontLeft.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    lblBreakTemp.Text = data.BrakeTemp.FrontRight.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    lblBreakTemp.Text = data.BrakeTemp.RearLeft.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    lblBreakTemp.Text = data.BrakeTemp.RearRight.ToString("0");
                    break;

            }
        }

        private void UpdatePressureControl(R3ESharedData data)
        {            
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    lblTyrePressure.Text = data.TirePressure.FrontLeft.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    lblTyrePressure.Text = data.TirePressure.FrontRight.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    lblTyrePressure.Text = data.TirePressure.RearLeft.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    lblTyrePressure.Text = data.TirePressure.RearRight.ToString("0");
                    break;

            }
        }

        private void UpdateWheelTemp(R3ESharedData data)
        {
            switch (WheelPostion)
            {
                case WheelPostionEnum.FrontLeft:
                    wheelTempLeft.Text = data.TireTemp.FrontLeft_Left.ToString("0");
                    wheelTempCenter.Text = data.TireTemp.FrontLeft_Center.ToString("0");
                    wheelTempRight.Text = data.TireTemp.FrontLeft_Right.ToString("0");
                    break;
                case WheelPostionEnum.FrontRight:
                    wheelTempLeft.Text = data.TireTemp.FrontRight_Left.ToString("0");
                    wheelTempCenter.Text = data.TireTemp.FrontRight_Center.ToString("0");
                    wheelTempRight.Text = data.TireTemp.FrontRight_Right.ToString("0");
                    break;
                case WheelPostionEnum.RearLeft:
                    wheelTempLeft.Text = data.TireTemp.RearLeft_Left.ToString("0");
                    wheelTempCenter.Text = data.TireTemp.RearLeft_Center.ToString("0");
                    wheelTempRight.Text = data.TireTemp.RearLeft_Right.ToString("0");
                    break;
                case WheelPostionEnum.RearRight:
                    wheelTempLeft.Text = data.TireTemp.RearRight_Left.ToString("0");
                    wheelTempCenter.Text = data.TireTemp.RearRight_Center.ToString("0");
                    wheelTempRight.Text = data.TireTemp.RearRight_Right.ToString("0");
                    break;

            }
        }

        public void UpdateControl(R3ESharedData data)
        {
            UpdateBrakeControl(data);
            UpdateWheelTemp(data);
            UpdatePressureControl(data);
        }

        public WheelStatusControl()
        {            
            InitializeComponent();
        }
    }
}
