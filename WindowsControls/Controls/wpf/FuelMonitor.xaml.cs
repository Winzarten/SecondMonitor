using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
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

        private static readonly Volume FuelConsumedMaximumThreshold = Volume.FromLiters(1);
        private static readonly double DistanceMaxThreshold = 300;
        public enum FuelOutputEnum { Time, Laps}
        public VolumeUnits DisplayUnits { get; set; } = VolumeUnits.UsGallons;
        private TimeSpan _lastSessionTime;
        private Volume _averageConsumptionPerLap;
        private Volume _averageConsmptionPerMinute;
        private Volume _lastFuelState = Volume.FromLiters(0);
        private Volume _totalFuelConsumed = Volume.FromLiters(0);
        private TimeSpan _timeLeft;
        Volume _fuelPerMinute;
        private Volume _fuelPerTickDistance;
        private double _totalTime;
        private double _totalLapDistanceCovered;
        private double _lastLapDistance;
        private double _remainingLaps;
        public FuelMonitor()
        {
            InitializeComponent();
            ResetFuelMonitor();
        }

        public void ResetFuelMonitor()
        {
            _lastFuelState = Volume.FromLiters(0);
            _totalFuelConsumed = Volume.FromLiters(0);
            _fuelPerTickDistance = Volume.FromLiters(0);
            _averageConsumptionPerLap = Volume.FromLiters(0);
            _averageConsmptionPerMinute = Volume.FromLiters(0);
            _fuelPerMinute = Volume.FromLiters(0);
            _totalTime = 0;
            _totalLapDistanceCovered = 0;
            _lastLapDistance = 0;
            _remainingLaps = 0;
            _lastSessionTime = new TimeSpan(0);
            if ((bool)rbtLaps.IsChecked)
                OutputType = FuelOutputEnum.Laps;
            if ((bool)rbtTime.IsChecked)
                OutputType = FuelOutputEnum.Time;
        }

        public FuelOutputEnum OutputType { get; set; }

        public void ProcessDataSet(SimulatorDataSet set)
        {
            if (set.PlayerInfo == null)
                return;
            fuelGauge.Value = (float)((set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.InLiters / set.PlayerInfo.CarInfo.FuelSystemInfo.FuelCapacity.InLiters) * 100);
            lblFuel.Content = "Total("+Volume.GetUnitSymbol(DisplayUnits)+"): "+ set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining.GetValueInUnits(DisplayUnits).ToString("N2");

            Volume fuelLeft = set.PlayerInfo.CarInfo.FuelSystemInfo.FuelRemaining;
            Volume fuelConsumed = _lastFuelState - fuelLeft;
            
            var tickDistanceCovered = set.PlayerInfo.LapDistance - _lastLapDistance;
            // ReSharper disable once CompareOfFloatsByEqualityOperator, because its initialization check
            if (_lastLapDistance != 0)
            {                
                if (tickDistanceCovered < 0)
                    tickDistanceCovered += set.SessionInfo.LayoutLength;
                if (tickDistanceCovered < DistanceMaxThreshold)
                    _totalLapDistanceCovered += tickDistanceCovered;
            }
            if (!set.PlayerInfo.InPits && _lastSessionTime.Ticks != 0 && fuelConsumed < FuelConsumedMaximumThreshold && tickDistanceCovered < DistanceMaxThreshold && tickDistanceCovered > 0.01 && (fuelConsumed.InLiters > 0 || _totalLapDistanceCovered > 0.01))
            {
                double timeSpan = set.SessionInfo.SessionTime.TotalMilliseconds - _lastSessionTime.TotalMilliseconds;
                _totalFuelConsumed += fuelConsumed;
                _totalTime += timeSpan;
                UpdateAsTime(set, fuelLeft, fuelConsumed, timeSpan);
                UpdateAsLaps(set, fuelLeft, fuelConsumed, tickDistanceCovered);
            }
            DisplayConsumption();
            _lastFuelState = fuelLeft;
            _lastSessionTime = set.SessionInfo.SessionTime;
            if (set.PlayerInfo == null)
                return;
                        
            _lastLapDistance = set.PlayerInfo.LapDistance;
        }

        private void UpdateAsLaps(SimulatorDataSet set, Volume fuelLeft, Volume fuelConsumed, double tickDistnace)
        {
            double ticksToLap = set.SessionInfo.LayoutLength / tickDistnace;
            _fuelPerTickDistance = fuelConsumed * ticksToLap;
            double totalLapsCovered = _totalLapDistanceCovered / set.SessionInfo.LayoutLength;
            _averageConsumptionPerLap = _totalFuelConsumed / totalLapsCovered;
            _remainingLaps = fuelLeft.InLiters/ _averageConsumptionPerLap.InLiters;

        }

        private void UpdateAsTime(SimulatorDataSet set, Volume fuelLeft, Volume fuelConsumed, double timeSpan)
        {
            double ticksPerSecond = 1000 / timeSpan;
            _fuelPerMinute = fuelConsumed * ticksPerSecond * 60;
            _averageConsmptionPerMinute = (_totalFuelConsumed / _totalTime) * 60000;
            double remaining = fuelLeft.InLiters / _averageConsmptionPerMinute.InLiters;
            if(!double.IsInfinity(remaining) && !double.IsNaN(remaining))
                _timeLeft = TimeSpan.FromMinutes(remaining);
        }

        private void DisplayConsumption()
        {
            if (OutputType == FuelOutputEnum.Time)
            {
                lblConsumtion.Content = "Rate(" + Volume.GetUnitSymbol(DisplayUnits) + "/m):" + _fuelPerMinute.GetValueInUnits(DisplayUnits).ToString("N2");
                //lblConsumtion.Content = totalFuelConsumed.InLiters.ToString("N2");
                lblAverage.Content = "Avg(" + Volume.GetUnitSymbol(DisplayUnits) + "/ m):" + _averageConsmptionPerMinute.GetValueInUnits(DisplayUnits).ToString("N2");
                var output = $"Time Left:{(int)_timeLeft.TotalMinutes}:{_timeLeft.Seconds:00}";
                lblRemaining.Content = output;
            }else
            {
                lblConsumtion.Content = "Rate (" + Volume.GetUnitSymbol(DisplayUnits) + "/lap):" + _fuelPerTickDistance.GetValueInUnits(DisplayUnits).ToString("N2");
                lblAverage.Content = "Avg (" + Volume.GetUnitSymbol(DisplayUnits) + "/ lap):" + _averageConsumptionPerLap.GetValueInUnits(DisplayUnits).ToString("N2");
                var output = "Laps left:" + _remainingLaps.ToString("N2");
                lblRemaining.Content = output;
            }
        }

        private void rbtMode_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rbtLaps.IsChecked)
                OutputType = FuelOutputEnum.Laps;
            if((bool)rbtTime.IsChecked)
                OutputType = FuelOutputEnum.Time;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetFuelMonitor();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (OutputType == FuelOutputEnum.Time)
            {
                Volume requiredFuel = (int)upDownDistance.Value * _averageConsmptionPerMinute;
                txtFuel.Text = requiredFuel.GetValueInUnits(DisplayUnits).ToString("N1")+Volume.GetUnitSymbol(DisplayUnits);
            }
            else
            {
                Volume requiredFuel = (int)upDownDistance.Value * _averageConsumptionPerLap;
                txtFuel.Text = requiredFuel.GetValueInUnits(DisplayUnits).ToString("N1") + Volume.GetUnitSymbol(DisplayUnits);
            }
        }
    }
}
