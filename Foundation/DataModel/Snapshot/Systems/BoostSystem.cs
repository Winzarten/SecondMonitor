namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using BasicProperties;

    [Serializable]
    public class BoostSystem
    {
        public BoostStatus BoostStatus { get; set; } = BoostStatus.UnAvailable;

        public TimeSpan CooldownTimer { get; set; } = TimeSpan.Zero;

        public TimeSpan TimeRemaining { get; set; } = TimeSpan.Zero;

        public int ActivationsRemaining { get; set; } = -1;
    }
}