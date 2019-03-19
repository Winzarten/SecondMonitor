namespace SecondMonitor.DataModel.Snapshot.Systems
{
    using BasicProperties;

    public class DrsSystem
    {
        public DrsStatus DrsStatus { get; set; }
        public int DrsActivationLeft { get; set; } = -1;
    }
}