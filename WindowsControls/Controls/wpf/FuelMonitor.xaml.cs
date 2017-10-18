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

        public enum FuelOutputEnum { TIME, LAPS}
        private TimeSpan lastSessionTime;
        private double lastFuelState;
        private double totalFuelConsumed;
        private double totalTime;
        private double totalLapDistanceCovered;
        private double lastLapDistance;
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
            totalLapDistanceCovered = 0;
            lastLapDistance = 0;
            lastSessionTime = new TimeSpan(0);
            if ((bool)rbtLaps.IsChecked)
                OutputType = FuelOutputEnum.LAPS;
            if ((bool)rbtTime.IsChecked)
                OutputType = FuelOutputEnum.TIME;
        }

        public FuelOutputEnum OutputType { get; set; }

        public void ProcessDataSet(SimulatorDataSet set)
        {
            if (set.PlayerInfo == null)
                return;
            fuelGauge.Value = (float)((set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters / set.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity.InLiters) * 100);
            lblFuel.Content = "Total(L): "+ set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters.ToString("N2");

            double fuelLeft = set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters;
            double fuelConsumed = lastFuelState - fuelLeft;
            
            var tickDistanceCovered = set.PlayerInfo.LapDistance - lastLapDistance;
            if (lastLapDistance != 0)
            {                
                if (tickDistanceCovered < 0)
                    tickDistanceCovered += set.SessionInfo.LayoutLength;
                if (tickDistanceCovered < 100)
                    totalLapDistanceCovered += tickDistanceCovered;
            }
            if (lastSessionTime.Ticks != 0 && fuelConsumed>0 && fuelConsumed < 1 && totalLapDistanceCovered > 0)
            {
                double timeSpan = set.SessionInfo.SessionTime.TotalMilliseconds - lastSessionTime.TotalMilliseconds;
                totalFuelConsumed += fuelConsumed;
                totalTime += timeSpan;
                if (OutputType == FuelOutputEnum.TIME)
                    UpdateAsTime(set, fuelLeft, fuelConsumed, timeSpan);
                if (OutputType == FuelOutputEnum.LAPS)
                    UpdateAsLaps(set, fuelLeft, fuelConsumed, tickDistanceCovered);

            }

            lastFuelState = fuelLeft;
            lastSessionTime = set.SessionInfo.SessionTime;
            if (set.PlayerInfo == null)
                return;
                        
            lastLapDistance = set.PlayerInfo.LapDistance;            
        }

        private void UpdateAsLaps(SimulatorDataSet set, double fuelLeft, double fuelConsumed, double tickDistnace)
        {
            double ticksToLap = set.SessionInfo.LayoutLength / tickDistnace;
            double fuelPerTickDistance = fuelConsumed * ticksToLap;
            lblConsumtion.Content = "Rate (l/lap):" + fuelPerTickDistance.ToString("N2");

            double totalLapsCovered = totalLapDistanceCovered / set.SessionInfo.LayoutLength;
            double averageConsmption = totalFuelConsumed / totalLapsCovered;
            lblAverage.Content = "Avg (l/lap):" + averageConsmption.ToString("N2");

            double remaining = fuelLeft / averageConsmption;
            var output = "L:" + remaining.ToString("N2");
            lblRemaining.Content = output;
        }

        private void UpdateAsTime(SimulatorDataSet set, double fuelLeft, double fuelConsumed, double timeSpan)
        {            
            double ticksPerSecond = 1000 / timeSpan;
            double fuelPerMinute = fuelConsumed * ticksPerSecond * 60;
            lblConsumtion.Content = "Rate(l/m):" + fuelPerMinute.ToString("N2");
           
            double averageConsmption = (totalFuelConsumed / totalTime) * 60000;
            lblAverage.Content = "Avg(l/m):" + averageConsmption.ToString("N2");

            double remaining = fuelLeft / averageConsmption;
            TimeSpan timeLeft = TimeSpan.FromMinutes(remaining);
            var output = $"Time Left:{(int)timeLeft.TotalMinutes}:{timeLeft.Seconds:00}";
            lblRemaining.Content = output;
        }

        private void rbtMode_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rbtLaps.IsChecked)
                OutputType = FuelOutputEnum.LAPS;
            if((bool)rbtTime.IsChecked)
                OutputType = FuelOutputEnum.TIME;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetFuelMonitor();
        }
    }
}
