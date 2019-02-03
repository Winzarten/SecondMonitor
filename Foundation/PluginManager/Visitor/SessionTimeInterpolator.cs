namespace SecondMonitor.PluginManager.Visitor
{
    using System;
    using System.Diagnostics;
    using DataModel.Snapshot;

    public class SessionTimeInterpolator : ISimulatorDateSetVisitor
    {
        private readonly TimeSpan _maxInterpolation;
        private readonly Stopwatch _stopwatch;
        private TimeSpan _lastInterpolation;
        private TimeSpan _lastSessionTime;

        public SessionTimeInterpolator(TimeSpan maxInterpolation)
        {
            _maxInterpolation = maxInterpolation;
            _stopwatch = new Stopwatch();
            Reset();
        }

        public void Visit(SimulatorDataSet simulatorDataSet)
        {
            if (simulatorDataSet.SessionInfo.SessionTime != _lastSessionTime)
            {
                _lastSessionTime = simulatorDataSet.SessionInfo.SessionTime;
                _stopwatch.Restart();
                return;
            }


            simulatorDataSet.SimulatorSourceInfo.TimeInterpolated = true;
            TimeSpan elapsed = _stopwatch.Elapsed;
            if (elapsed >= _maxInterpolation)
            {
                _stopwatch.Stop();
                _lastInterpolation = _maxInterpolation;
            }
            else
            {
                _lastInterpolation = elapsed;
            }

            //simulatorDataSet.SessionInfo.SessionTime = simulatorDataSet.SessionInfo.SessionTime + _lastInterpolation;
        }

        public TimeSpan ApplyInterpolation(TimeSpan timeSpan)
        {
            return timeSpan;// + _lastInterpolation;
        }

        public void Reset()
        {
            _stopwatch.Restart();
            _lastSessionTime = TimeSpan.Zero;
        }
    }
}