namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Windows.Media;

    public class LapColorArgs : EventArgs
    {
        public LapColorArgs(string lapId, Color color)
        {
            LapId = lapId;
            Color = color;
        }

        public string LapId { get; set; }
        public Color Color { get; set; }
    }
}