namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using System;
    using BasicProperties;

    [Serializable]
    public class DrsSystem
    {
        public DrsStatus DrsStatus { get; set; }
        public int DrsActivationLeft { get; set; } = -1;
    }
}