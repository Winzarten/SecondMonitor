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
        public void TestExportSummaryLaps()
        {
            // Arrange
            string fileName = @"Laps.xlsx";
            SessionSummary sessionSummary = GetBlankSessionSummary();
            sessionSummary.TotalNumberOfLaps = 20;
            sessionSummary.SessionLengthType = SessionLengthType.Laps;
            AddDrivers(20, sessionSummary);
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

        [Test]
        public void TestExportSummaryTime()
        {
            // Arrange
            string fileName = @"time.xlsx";
            SessionSummary sessionSummary = GetBlankSessionSummary();
            sessionSummary.SessionLength = TimeSpan.FromMinutes(90);
            sessionSummary.SessionLengthType = SessionLengthType.Time;
            AddDrivers(20, sessionSummary);
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

        private static void AddDrivers(int driverCount, SessionSummary sessionSummary)
        {
            for (int i = 0; i < driverCount; i++)
            {
                sessionSummary.Drivers.Add(new Driver()
                                                {
                                                    CarName = "A caaaar",
                                                    DriverName = "Driver " + i,
                                                    TotalLaps = 20
                                                });
            }
        }

        private static SessionSummary GetBlankSessionSummary()
        {
            return new SessionSummary()
                       {
                           SessionPhase = SessionPhase.Green,
                           SessionRunTime = DateTime.Now,
                           SessionType = SessionType.Race,
                           Simulator = "R3E",
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