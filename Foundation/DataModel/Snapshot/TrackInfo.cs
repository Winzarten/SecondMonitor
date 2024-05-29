namespace SecondMonitor.DataModel.Snapshot
{
    using System;
    using System.Xml.Serialization;
    using BasicProperties;

    [Serializable]
    public sealed class TrackInfo
    {
        public TrackInfo()
        {
            TrackName = string.Empty;
            TrackLayoutName = string.Empty;
        }

        public string TrackName { get; set; }

        public string TrackLayoutName { get; set; }

        public Distance LayoutLength { get; set; }

        [XmlIgnore]
        public string TrackFullName => string.IsNullOrEmpty(TrackLayoutName) ? TrackName : $"{TrackName}-{TrackLayoutName}";
    }
}