namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SnapshotSection
{
    using DataModel.BasicProperties;

    public interface IPedalSectionViewModel : ISnapshotViewModel
    {
        double ThrottlePosition { get; set; }

        double BrakePosition { get; set; }

        double ClutchPosition { get; set; }

        string Gear { get; set; }

        double SteerAngle { get; set; }

        int Rpm { get; set; }

        Velocity Speed { get; set; }

        VelocityUnits VelocityUnits { get; set; }
    }
}