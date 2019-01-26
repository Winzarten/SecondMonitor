namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    public class LapColorSynchronization : ILapColorSynchronization
    {
        private readonly Dictionary<string, Color> _lapColors;
        public event EventHandler<LapColorArgs> LapColorChanged;

        public LapColorSynchronization()
        {
            _lapColors = new Dictionary<string, Color>();
        }

        public bool TryGetColorForLap(string lapId, out Color color)
        {
            return _lapColors.TryGetValue(lapId, out color);
        }

        public void SetColorForLap(string lapId, Color color)
        {
            _lapColors[lapId] = color;
            LapColorChanged?.Invoke(this, new LapColorArgs(lapId, color));
        }
    }
}