namespace SecondMonitor.Timing.ReportCreation.ViewModel
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts.Commands;
    using DataModel.BasicProperties;
    using DataModel.Summary;

    using NLog;

    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using ViewModels.Settings.ViewModel;
    using XslxExport;

    public class ReportsController
    {

        private const string ReportNamePrefix = "Report_";

        private const string ReportDirectoryName = "Reports";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReportsController(DisplaySettingsViewModel settingsView)
        {
            SettingsView = settingsView;
            OpenLastReportCommand = new RelayCommand(OpenLastReport);
            OpenReportsFolderCommand = new RelayCommand(OpenReportsFolder);
        }

        public RelayCommand OpenLastReportCommand { get; }

        public RelayCommand OpenReportsFolderCommand { get; }

        public DisplaySettingsViewModel SettingsView { get; set; }

        public void CreateReport(SessionTiming sessionTiming)
        {
            if (!ShouldBeExported(sessionTiming))
            {
                return;
            }
            try
            {
                SessionSummary sessionSummary = sessionTiming.ToSessionSummary();
                string reportName = GetReportName(sessionSummary);
                SessionSummaryExporter sessionSummaryExporter = CreateSessionSummaryExporter();
                string fullReportPath = Path.Combine(
                    GetReportDirectory(),
                    reportName);
                sessionSummaryExporter.ExportSessionSummary(sessionSummary, fullReportPath);
                OpenReportIfEnabled(sessionSummary, fullReportPath);
                CheckAndDeleteIfMaximumReportsExceeded();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to export session info");
            }
        }

        public void OpenLastReport()
        {
            DirectoryInfo di = new DirectoryInfo(GetReportDirectory());
            FileInfo fileToOpen = di.GetFiles().OrderBy(x => x.CreationTime).LastOrDefault();
            if (fileToOpen == null)
            {
                return;
            }

            OpenReport(fileToOpen.FullName);
        }

        public void OpenReportsFolder()
        {
            string reportDirectory = GetReportDirectory();
            Task.Run(
                () =>
                    {
                        Process.Start(reportDirectory);
                    });
        }

        private SessionSummaryExporter CreateSessionSummaryExporter()
        {
            SessionSummaryExporter sessionSummaryExporter = new SessionSummaryExporter();
            sessionSummaryExporter.VelocityUnits = SettingsView.VelocityUnits;
            return sessionSummaryExporter;
        }

        private bool ShouldBeExported(SessionTiming sessionTiming)
        {
            if (sessionTiming?.LastSet?.SessionInfo == null || SettingsView == null)
            {
                return false;
            }

            if (sessionTiming.LastSet.SessionInfo.SessionTime.TotalMinutes < SettingsView.ReportingSettingsView.MinimumSessionLength)
            {
                return false;
            }

            switch (sessionTiming.SessionType)
            {
                case SessionType.Practice:
                    return SettingsView.ReportingSettingsView.PracticeReportSettingsView.Export;
                case SessionType.Qualification:
                    return SettingsView.ReportingSettingsView.QualificationReportSettingView.Export;
                case SessionType.WarmUp:
                    return SettingsView.ReportingSettingsView.WarmUpReportSettingsView.Export;
                case SessionType.Race:
                    return SettingsView.ReportingSettingsView.RaceReportSettingsView.Export;
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
                    openReport = SettingsView.ReportingSettingsView.PracticeReportSettingsView.AutoOpen;
                    break;
                case SessionType.Qualification:
                    openReport = SettingsView.ReportingSettingsView.QualificationReportSettingView.AutoOpen;
                    break;
                case SessionType.WarmUp:
                    openReport = SettingsView.ReportingSettingsView.WarmUpReportSettingsView.AutoOpen;
                    break;
                case SessionType.Race:
                    openReport = SettingsView.ReportingSettingsView.RaceReportSettingsView.AutoOpen;
                    break;
            }
            if (!openReport)
            {
                return;
            }

            OpenReport(reportPath);
        }

        private void OpenReport(string reportPath)
        {
            Task.Run(() => { Process.Start(reportPath); });
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
            DirectoryInfo di = new DirectoryInfo(GetReportDirectory());
            FileInfo[] files = di.GetFiles(ReportNamePrefix + "*.xlsx").OrderBy(f => f.CreationTimeUtc).ToArray();
            if (files.Length <= SettingsView.ReportingSettingsView.MaximumReports)
            {
                return;
            }

            int filesToDelete = files.Length - SettingsView.ReportingSettingsView.MaximumReports;
            for (int i = 0; i < filesToDelete; i++)
            {
                files[i].Delete();
            }
        }

        private string GetReportDirectory()
        {
            string directoryPath = Path.Combine(SettingsView.ReportingSettingsView.ExportDirectoryReplacedSpecialDirs, ReportDirectoryName);
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }
    }
}