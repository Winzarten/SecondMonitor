namespace SecondMonitor.DataModel.Snapshot
{
    public class TrackInfo
    {
        public string TrackName { get; set; }

        public string TrackLayoutName { get; set; }

        public double LayoutLength { get; set; }

        public TrackInfo()
        {
            TrackName = string.Empty;
            TrackLayoutName = string.Empty;
        }
    }
}