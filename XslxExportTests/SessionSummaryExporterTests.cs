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

        private Random _random;

        [SetUp]
        public void Setup()
        {
            this._random = new Random();
            this._testee = new SessionSummaryExporter() { VelocityUnits = VelocityUnits.Kph };
        }

        [Test]
        public void TestExportSummaryLaps()
        {
            // Arrange
            string fileName = @"Laps.xlsx";
            SessionSummary sessionSummary = GetBlankSessionSummary();
            sessionSummary.TotalNumberOfLaps = 20;
            sessionSummary.SessionLengthType = SessionLengthType.Laps;
            AddDrivers(40, sessionSummary);
            sessionSummary.Drivers.ForEach((s) => FillLaps(s, 140, 20));

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

        private void FillLaps(Driver driver, int baseTimeSeconds, int totalLaps)
        {
            if (!driver.Finished)
            {
                totalLaps = this._random.Next(1, totalLaps);
                driver.TotalLaps = totalLaps;
            }
            for (int i = 1; i <= totalLaps; i++)
            {
                double sectorBase = (double)baseTimeSeconds / 3;
                double sector1Add = this._random.NextDouble() * 10;
                double sector2Add = this._random.NextDouble() * 10;
                double sector3Add = this._random.NextDouble() * 10;
                driver.Laps.Add(new Lap(driver)
                {
                    LapNumber = i,
                    LapTime = TimeSpan.FromSeconds(baseTimeSeconds + sector1Add + sector2Add + sector3Add),
                    Sector1 = TimeSpan.FromSeconds(sectorBase + sector1Add),
                    Sector2 = TimeSpan.FromSeconds(sectorBase + sector2Add),
                    Sector3 = TimeSpan.FromSeconds(sectorBase + sector3Add)
                });
            }
        }

        private void AddDrivers(int driverCount, SessionSummary sessionSummary)
        {
            for (int i = 0; i < driverCount; i++)
            {
                sessionSummary.Drivers.Add(new Driver()
                                                {
                                                    CarName = "A caaaar",
                                                    DriverName = "Driver " + i,
                                                    TotalLaps = 20,
                                                    TopSpeed = Velocity.FromKph(this._random.Next(150,250)),
                                                    FinishingPosition = i + 1,
                                                    Finished = this._random.Next(0, 10) > 0 ? true : false
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