namespace SecondMonitor.WindowsControls.WinForms
{
    using System.Windows.Forms;

    using SecondMonitor.DataModel;

    public partial class OilControl : UserControl
    {
        public OilControl()
        {
            InitializeComponent();
        }

        private void gPressure_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        public void UpdateControl(SimulatorDataSet data)
        {
            if (data.PlayerInfo == null)
                return;
            gTemperature.Value = (float) data.PlayerInfo.CarInfo.OilSystemInfo.OilTemperature.InCelsius;
            gPressure.Value = (float) data.PlayerInfo.CarInfo.OilSystemInfo.OilPressure.InAtmospheres;
            lblPressure.Text = data.PlayerInfo.CarInfo.OilSystemInfo.OilPressure.InAtmospheres.ToString("0.0");
        }
    }
}
