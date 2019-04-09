namespace SecondMonitor.Telemetry.TelemetryApplication.Histogram
{
    public interface IWheelHistogramDataProvider
    {
        Histogram GetHistogramFrontLeft(double bandSize);
        Histogram GetHistogramFrontRight(double bandSize);
        Histogram GetHistogramRearLeft(double bandSize);
        Histogram GetHistogramRearRight(double bandSize);
    }
}