using SecondMonitor.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecondMonitor.WindowsControls.Controls.wpf
{
    /// <summary>
    /// Interaction logic for FuelMonitor.xaml
    /// </summary>    
    public partial class FuelMonitor : UserControl
    {
        private TimeSpan lastSessionTime;
        private double lastFuelState;
        private double totalFuelConsumed;
        private double totalTime;
        public FuelMonitor()
        {
            InitializeComponent();
            ResetFuelMonitor();
        }

        public void ResetFuelMonitor()
        {
            lastFuelState = 0;
            totalFuelConsumed = 0;
            totalTime = 0;
            lastSessionTime = new TimeSpan(0);
        }

        public void ProcessDataSet(SimulatorDataSet set)
        {
            fuelGauge.Value = (float)((set.PlayerCarInfo.FuelSystemInfo.FuelRemaining.InLiters / set.PlayerCarInfo.FuelSystemInfo.FuelCapacity.InLiters) * 100);
            lblFuel.Content = "Total: "+ set.PlayerCarInfo.FuelSystemInfo.FuelRemaining.InLiters.ToString("N2");

            double fuelLeft = set.PlayerCarInfo.FuelSystemInfo.FuelRemaining.InLiters;
            double fuelConsumed = lastFuelState - fuelLeft;
            if (lastSessionTime.Ticks != 0 && fuelConsumed>0 && fuelConsumed < 1)
            {
                
                double timeSpan = set.SessionInfo.SessionTime.TotalMilliseconds - lastSessionTime.TotalMilliseconds;
                double ticksPerSecond = 1000 / timeSpan;
                double fuelPerMinute = fuelConsumed * ticksPerSecond * 60;
                lblConsumtion.Content = "Rate:" + fuelPerMinute.ToString("N2");

                totalFuelConsumed += fuelConsumed;
                totalTime += timeSpan;

                double averageConsmption = (totalFuelConsumed / totalTime) * 60000;                
                lblAverage.Content = "Avg:" + averageConsmption.ToString("N2");

                double remaining = fuelLeft / averageConsmption;
                TimeSpan timeLeft = TimeSpan.FromMinutes(remaining);
                var output = $"{(int)timeLeft.TotalMinutes}:{timeLeft.Seconds:00}";
                lblRemaining.Content = output;
            }

            lastFuelState = fuelLeft;
            lastSessionTime = set.SessionInfo.SessionTime;

        }
    }
}
