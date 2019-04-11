namespace SecondMonitor.Telemetry.TelemetryApplication.AggregatedCharts
{
    using Histogram;
    using ViewModels.GraphPanel;
    using ViewModels.GraphPanel.Histogram;

    public class SuspensionVelocityHistogramProvider : IAggregatedChartProvider
    {
        private readonly SuspensionVelocityHistogramDataExtractor _suspensionVelocityHistogramDataExtractor;
        public string ChartName => "Suspension Velocity";

        public SuspensionVelocityHistogramProvider(SuspensionVelocityHistogramDataExtractor suspensionVelocityHistogramDataExtractor)
        {
            _suspensionVelocityHistogramDataExtractor = suspensionVelocityHistogramDataExtractor;
        }

        public AggregatedChartViewModel CreateAggregatedChartViewModel()
        {
            Histogram.Histogram flHistogram = _suspensionVelocityHistogramDataExtractor.ExtractHistogramFrontLeft();
            Histogram.Histogram frHistogram = _suspensionVelocityHistogramDataExtractor.ExtractHistogramFrontRight();
            Histogram.Histogram rlHistogram = _suspensionVelocityHistogramDataExtractor.ExtractHistogramRearLeft();
            Histogram.Histogram rrHistogram = _suspensionVelocityHistogramDataExtractor.ExtractHistogramRearRight();

            WheelsHistogramViewModel wheelsHistogram = new WheelsHistogramViewModel()
            {
                Title = ChartName,
            };

            wheelsHistogram.FrontLeftChartViewModel.FromModel(flHistogram);
            wheelsHistogram.FrontRightChartViewModel.FromModel(frHistogram);
            wheelsHistogram.RearLeftChartViewModel.FromModel(rlHistogram);
            wheelsHistogram.RearRightChartViewModel.FromModel(rrHistogram);

            return wheelsHistogram;
        }
    }
}