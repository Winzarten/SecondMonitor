namespace SecondMonitor.Timing.SessionTiming.Drivers.ModelView
{
    using System;

    using SecondMonitor.DataModel;
    using SecondMonitor.DataModel.Snapshot;

    public class PitStopInfo
    {
        public enum PitPhase { Entry, InPits, Exit, Completed }

        public PitStopInfo(SimulatorDataSet set, DriverTiming driver, LapInfo entryLap)
        {
            Driver = driver;
            EntryLap = entryLap;
            Phase = PitPhase.Entry;
            PitEntry = set.SessionInfo.SessionTime;
            PitStopDuration = TimeSpan.Zero;
            PitExit = PitEntry;
            PitStopStart = PitEntry;
            PitStopEnd = PitEntry;
        }

        public PitPhase Phase { get; private set; }

        public bool Completed => Phase == PitPhase.Completed;

        public DriverTiming Driver { get; }

        public LapInfo EntryLap { get; }

        public TimeSpan PitEntry { get; }

        public TimeSpan PitExit { get; private set; }

        public TimeSpan PitStopStart { get; private set; }

        public TimeSpan PitStopEnd { get; private set; }

        public TimeSpan PitStopDuration { get; private set; }

        public void Tick(SimulatorDataSet set)
        {
            if (Phase == PitPhase.Completed)
            {
                return;
            }

            if (Phase == PitPhase.Entry && Driver.DriverInfo.Speed.InKph < 1)
            {
                Phase = PitPhase.InPits;
                PitStopStart = set.SessionInfo.SessionTime;
            }

            if (Phase == PitPhase.InPits && Driver.DriverInfo.Speed.InKph > 1)
            {
                Phase = PitPhase.Exit;
                PitStopEnd = set.SessionInfo.SessionTime;
            }

            if (Phase == PitPhase.Exit && !Driver.DriverInfo.InPits)
            {
                Phase = PitPhase.Completed;
                PitExit = set.SessionInfo.SessionTime;
                PitStopDuration = PitExit.Subtract(PitEntry);
            }

            if(Phase == PitPhase.Entry)
            {
                PitStopStart = set.SessionInfo.SessionTime;
                PitStopEnd = set.SessionInfo.SessionTime;
            }

            if(Phase == PitPhase.InPits)
            {
                PitStopEnd = set.SessionInfo.SessionTime;
            }

            if (Phase != PitPhase.Completed)
            {
                PitExit = set.SessionInfo.SessionTime;
                PitStopDuration = PitExit.Subtract(PitEntry);
            }
        }

        public string PitInfoFormatted
        {
            get
            {
                string phaseAsString = string.Empty;
                switch(Phase)
                {
                    case PitPhase.Entry:
                        phaseAsString = "Entry-";
                        break;
                    case PitPhase.InPits:
                        phaseAsString = "Stop-";
                        break;
                    case PitPhase.Exit:
                        phaseAsString = "Exit-";
                        break;
                    case PitPhase.Completed:
                        phaseAsString = EntryLap != null ? EntryLap.LapNumber + "-": "0-";
                        break;
                    default:
                        return string.Empty;
                }
                return phaseAsString + $"{(int) PitStopDuration.TotalSeconds}.{PitStopDuration.Milliseconds:N0}";
            }
        }

    }
}
