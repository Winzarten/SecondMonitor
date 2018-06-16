namespace SecondMonitor.Timing.ReportCreation.ViewModel
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Summary;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using SecondMonitor.Timing.Settings.ModelView;
    using SecondMonitor.XslxExport;

    public class ReportCreationViewModel
    {

        private const string ReportNamePrefix = "Report_";

        public ReportCreationViewModel(DisplaySettingsModelView settings)
        {
            Settings = settings;
        }

        public DisplaySettingsModelView Settings { get; set; }

        public void CreateReport(SessionTiming sessionTiming)
        {
            if (!ShouldBeExported(sessionTiming))
            {
                return;
            }

            SessionSummary sessionSummary = sessionTiming.ToSessionSummary();
            string reportName = GetReportName(sessionSummary);
            SessionSummaryExporter sessionSummaryExporter = CreateSessionSummaryExporter();
            string fullReportPath = Path.Combine(
                Settings.ReportingSettings.ExportDirectoryReplacedSpecialDirs,
                reportName);
            sessionSummaryExporter.ExportSessionSummary(sessionSummary,fullReportPath);
            OpenReportIfEnabled(sessionSummary, fullReportPath);
            CheckAndDeleteIfMaximumReportsExceeded();            
        }

        private SessionSummaryExporter CreateSessionSummaryExporter()
        {
            SessionSummaryExporter sessionSummaryExporter = new SessionSummaryExporter();
            sessionSummaryExporter.VelocityUnits = Settings.VelocityUnits;
            return sessionSummaryExporter;
        }

        private bool ShouldBeExported(SessionTiming sessionTiming)
        {
            if (sessionTiming.LastSet.SessionInfo.SessionTime.TotalMinutes < Settings.ReportingSettings.MinimumSessionLength)
            {
                return false;
            }

            switch (sessionTiming.SessionType)
            {
                case SessionType.Practice:
                    return Settings.ReportingSettings.PracticeReportSettings.Export;
                case SessionType.Qualification:
                    return Settings.ReportingSettings.QualificationReportSetting.Export;
                case SessionType.WarmUp:
                    return Settings.ReportingSettings.WarmUpReportSettings.Export;
                case SessionType.Race:
                    return Settings.ReportingSettings.RaceReportSettings.Export;
                default:
                    return false;
            }
        }

        private void OpenReportIfEnabled(SessionSummary sessionSummary, string reportPath)
        {
            bool openReport = false;
            switch (sessionSummary.SessionType)
            {
                case SessionType.Na:
                    openReport = false;
                    break;
                case SessionType.Practice:
                    openReport = Settings.ReportingSettings.PracticeReportSettings.AutoOpen;
                    break;
                case SessionType.Qualification:
                    openReport = Settings.ReportingSettings.QualificationReportSetting.AutoOpen;
                    break;
                case SessionType.WarmUp:
                    openReport = Settings.ReportingSettings.WarmUpReportSettings.AutoOpen;
                    break;
                case SessionType.Race:
                    openReport = Settings.ReportingSettings.RaceReportSettings.AutoOpen;
                    break;
            }
            if (!openReport)
            {
                return;
            }
            System.Diagnostics.Process.Start(reportPath);
        }

        private string GetReportName(SessionSummary sessionSummary)
        {
            StringBuilder reportName = new StringBuilder(ReportNamePrefix);
            
            reportName.Append(DateTime.Now.ToString("s") + "_").Replace(":","-");
            reportName.Append(sessionSummary.TrackInfo.TrackName + "_");
            reportName.Append(sessionSummary.SessionType);
            reportName.Append(".xlsx");
            return reportName.ToString();
        }

        private void CheckAndDeleteIfMaximumReportsExceeded()
        {
            DirectoryInfo di = new DirectoryInfo(Settings.ReportingSettings.ExportDirectoryReplacedSpecialDirs);
            FileInfo[] files = di.GetFiles(ReportNamePrefix + "*.xlsx").OrderBy(f => f.CreationTimeUtc).ToArray();
            if (files.Length <= Settings.ReportingSettings.MaximumReports)
            {
                return;
            }

            int filesToDelete = files.Length - Settings.ReportingSettings.MaximumReports;
            for (int i = 0; i < filesToDelete; i++)
            {
                files[i].Delete();
            }
        }
    }
}