namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.Histogram
{
    using System.Collections.Generic;
    using System.Linq;
    using TelemetryApplication.AggregatedCharts.Histogram;

    public class SuspensionVelocityHistogramChartViewModel : HistogramChartViewModel
    {
        private double _bumpAverageSpeed;
        private double _reboundAverageSpeed;
        private double _bumpPercentage;
        private double _reboundPercentage;

        public double BumpAverageSpeed
        {
            get => _bumpAverageSpeed;
            set => SetProperty(ref _bumpAverageSpeed, value);
        }

        public double ReboundAverageSpeed
        {
            get => _reboundAverageSpeed;
            set => SetProperty(ref _reboundAverageSpeed, value);
        }

        public double BumpPercentage
        {
            get => _bumpPercentage;
            set => SetProperty(ref _bumpPercentage, value);
        }

        public double ReboundPercentage
        {
            get => _reboundPercentage;
            set => SetProperty(ref _reboundPercentage, value);
        }

        protected override void ApplyModel(Histogram model)
        {
            List<HistogramBand> bumps = model.Items.Where(x => x.Category > 0).ToList();
            List<HistogramBand> rebound = model.Items.Where(x => x.Category < 0).ToList();
            BumpPercentage = bumps.Sum(x => x.Percentage);
            ReboundPercentage = rebound.Sum(x => x.Percentage);
            BumpAverageSpeed = bumps.Sum(x => x.Category * x.Percentage) / _bumpPercentage;
            ReboundAverageSpeed = rebound.Sum(x => x.Category * x.Percentage) / _reboundPercentage;
            base.ApplyModel(model);
        }
    }
}