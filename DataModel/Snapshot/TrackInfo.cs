namespace SecondMonitor.DataModel.Snapshot
{
    public class TrackInfo
    {
        public string TrackName { get; set; }

        public string TrackLayoutName { get; set; }

        public float LayoutLength { get; set; }

        public TrackInfo()
        {
            this.TrackName = string.Empty;
            this.TrackLayoutName = string.Empty;
        }
    }
}