namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using BasicProperties;

    [Serializable]
    public class TrackInfo
    {
        public TrackInfo()
        {
            TrackName = string.Empty;
            TrackLayoutName = string.Empty;
        }

        public string TrackName { get; set; }

        public string TrackLayoutName { get; set; }

        public Distance LayoutLength { get; set; }
    }
}