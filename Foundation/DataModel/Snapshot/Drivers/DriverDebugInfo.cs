namespace SecondMonitor.DataModel.Snapshot.Drivers
{
    using System;

    [Serializable]
    public sealed class DriverDebugInfo
    {
        public DriverDebugInfo()
        {

        }

        public double DistanceToPits { get; set; }
    }
}