namespace SecondMonitor.DataModel.Summary
{
    using SecondMonitor.DataModel.BasicProperties;

    public class Driver
    {
        public string DriverName { get; set; }

        public int FinishingPosition { get; set; }

        public string CarName { get; set; }

        public int TotalLaps { get; set; }

        public Velocity TopSpeed { get; set; } = Velocity.Zero;
    }
}