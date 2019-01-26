namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Windows.Media;

    public interface ILapColorSynchronization
    {
        event EventHandler<LapColorArgs> LapColorChanged;

        bool TryGetColorForLap(string lapId, out Color color);
        void SetColorForLap(string lapId, Color color);
    }
}