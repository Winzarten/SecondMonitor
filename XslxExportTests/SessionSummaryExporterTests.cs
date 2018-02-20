namespace SecondMonitor.XslxExportTests
{
    using System;
    using System.IO;

    using NUnit.Framework;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Snapshot;
    using SecondMonitor.DataModel.Summary;
    using SecondMonitor.XslxExport;

    [TestFixture]
    public class SessionSummaryExporterTests
    {
        private SessionSummaryExporter _testee;

        [SetUp]
        public void Setup()
        {
            this._testee = new SessionSummaryExporter();
        }

        [Test]
        public void TestExportSummary()
        {
            // Arrange
            string fileName = @"foo.xlsx";
            SessionSummary sessionSummary = GetBlankSessionSummary();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Act
            this._testee.ExportSessionSummary(sessionSummary, fileName);

            // Assert
            Assert.That(File.Exists(fileName), Is.True);
            System.Diagnostics.Process.Start(fileName);
        }

        private static SessionSummary GetBlankSessionSummary()
        {
            return new SessionSummary()
                       {
                           SessionLengthType = SessionLengthType.Laps,
                           SessionPhase = SessionPhase.Green,
                           SessionRunTime = DateTime.Now,
                           SessionType = SessionType.Race,
                           TrackInfo = new TrackInfo()
                                           {
                                               LayoutLength = 2000,
                                               TrackLayoutName = "Grand Prix",
                                               TrackName = "Red bull Ring"
                                           }
                       };
        }
    }
}