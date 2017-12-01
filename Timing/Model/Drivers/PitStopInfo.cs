using SecondMonitor.DataModel;
using System;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class PitStopInfo
    {
        public enum PitPhase { Entry, Inpits, Exit, Completed };

        public PitStopInfo(SimulatorDataSet set, DriverTiming driver, LapInfo entryLap)
        {
            this.Driver = driver;
            this.EntryLap = entryLap;
            this.Phase = PitPhase.Entry;
            this.PitEntry = set.SessionInfo.SessionTime;
            this.PitStopDuration = new TimeSpan(0);
            this.PitExit = PitEntry;
            this.PitStopStart = PitEntry;
            this.PitStopEnd = PitEntry;
        }
        public PitPhase Phase { get; private set; }
        public bool Completed { get => Phase == PitPhase.Completed; }
        public DriverTiming Driver { get; private set; }
        public LapInfo EntryLap { get; private set; }
        public TimeSpan PitEntry { get; private set; }
        public TimeSpan PitExit { get; private set; }
        public TimeSpan PitStopStart { get; private set; }
        public TimeSpan PitStopEnd { get; private set; }
        public TimeSpan PitStopDuration { get; private set; }

        public void Tick(SimulatorDataSet set)
        {
            if (Phase == PitPhase.Completed)
                return;
            if (Phase == PitPhase.Entry && Driver.DriverInfo.Speed.InKph < 1)
            {
                Phase = PitPhase.Inpits;
                PitStopStart = set.SessionInfo.SessionTime;
            }
            if (Phase == PitPhase.Inpits && Driver.DriverInfo.Speed.InKph > 1)
            {
                Phase = PitPhase.Exit;
                PitStopEnd = set.SessionInfo.SessionTime;
            }
            if (Phase == PitPhase.Exit && !Driver.DriverInfo.InPits)
            {
                Phase = PitPhase.Completed;
                PitExit = set.SessionInfo.SessionTime;
                this.PitStopDuration = PitExit.Subtract(PitEntry);
            }
            if(Phase == PitPhase.Entry)
            {
                this.PitStopStart = set.SessionInfo.SessionTime;
                this.PitStopEnd = set.SessionInfo.SessionTime; ;
            }
            if(Phase == PitPhase.Inpits)
            {
                this.PitStopEnd = set.SessionInfo.SessionTime;
            }
            if (Phase != PitPhase.Completed)
            {
                this.PitExit = set.SessionInfo.SessionTime;
                this.PitStopDuration = PitExit.Subtract(PitEntry);
            }
        }

        public string PitInfoFormatted
        {
            get
            {
                string phaseAsString = "";
                switch(Phase)
                {
                    case PitPhase.Entry:
                        phaseAsString = "Entry-";
                        break;
                    case PitPhase.Inpits:
                        phaseAsString = "Stop-";
                        break;
                    case PitPhase.Exit:
                        phaseAsString = "Exit-";
                        break;
                    case PitPhase.Completed:
                        phaseAsString = EntryLap != null ? EntryLap.LapNumber.ToString() + "-": "0-";
                        break;

                }
                
                return phaseAsString + $"{(int) PitStopDuration.TotalSeconds}.{PitStopDuration.Milliseconds:N0}";
            }
        }

    }
}
