using SecondMonitor.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondMonitor.Timing.Model.Drivers
{
    public class PitInfo
    {
        public enum PitPhase { ENTRY, INPITS, EXIT, COMPLETED };

        public PitInfo(SimulatorDataSet set, DriverTiming driver, LapInfo entryLap)
        {
            this.Driver = driver;
            this.EntryLap = entryLap;
            this.Phase = PitPhase.ENTRY;
            this.PitEntry = set.SessionInfo.SessionTime;
            this.PitStopDuration = new TimeSpan(0);
            this.PitExit = PitEntry;
            this.PitStopStart = PitEntry;
            this.PitStopEnd = PitEntry;
        }
        public PitPhase Phase { get; private set; }
        public bool Completed { get => Phase == PitPhase.COMPLETED; }
        public DriverTiming Driver { get; private set; }
        public LapInfo EntryLap { get; private set; }
        public TimeSpan PitEntry { get; private set; }
        public TimeSpan PitExit { get; private set; }
        public TimeSpan PitStopStart { get; private set; }
        public TimeSpan PitStopEnd { get; private set; }
        public TimeSpan PitStopDuration { get; private set; }

        public void Tick(SimulatorDataSet set)
        {
            if (Phase == PitPhase.COMPLETED)
                return;
            if (Phase == PitPhase.ENTRY && Driver.DriverInfo.Speed < 1)
            {
                Phase = PitPhase.INPITS;
                PitStopStart = set.SessionInfo.SessionTime;
            }
            if (Phase == PitPhase.INPITS && Driver.DriverInfo.Speed > 1)
            {
                Phase = PitPhase.EXIT;
                PitStopEnd = set.SessionInfo.SessionTime;
            }
            if (Phase == PitPhase.EXIT && !Driver.DriverInfo.InPits)
            {
                Phase = PitPhase.COMPLETED;
                PitExit = set.SessionInfo.SessionTime;
                this.PitStopDuration = PitExit.Subtract(PitEntry);
            }            
            if(Phase == PitPhase.ENTRY)
            {                                                
                this.PitStopStart = set.SessionInfo.SessionTime;
                this.PitStopEnd = set.SessionInfo.SessionTime; ;
            }
            if(Phase == PitPhase.INPITS)
            {
                this.PitStopEnd = set.SessionInfo.SessionTime;
            }
            if (Phase != PitPhase.COMPLETED)
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
                    case PitPhase.ENTRY:
                        phaseAsString = "Entry-";
                        break;
                    case PitPhase.INPITS:
                        phaseAsString = "Stop-";
                        break;
                    case PitPhase.EXIT:
                        phaseAsString = "Out-";
                        break;
                    case PitPhase.COMPLETED:
                        phaseAsString = EntryLap != null ? EntryLap.LapNumber.ToString() + "-": "0-";
                        break;

                }
                return phaseAsString + PitStopDuration.ToString("ss\\.fff");
            }
        }

    }
}
