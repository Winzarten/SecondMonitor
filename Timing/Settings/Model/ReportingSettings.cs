namespace SecondMonitor.Timing.Settings.Model
{
    using System;

    [Serializable]
    public class ReportingSettings
    {
        public string ExportDirectory { get; set; } = @"%MyDocuments%\SecondMonitor";

        public int MaximumReports { get; set; } = 20;

        public int MinimumSessionLength { get; set; } = 5;

        public SessionReportSettings PracticeReportSettings { get; set; } = new SessionReportSettings();

        public SessionReportSettings QualificationReportSettings { get; set; } = new SessionReportSettings();

        public SessionReportSettings RaceReportSettings { get; set; } = new SessionReportSettings();

        public SessionReportSettings WarmUpReportSettings { get; set; } = new SessionReportSettings();


    }
}