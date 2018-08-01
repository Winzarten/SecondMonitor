namespace SecondMonitor.XslxExport
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;

    using NLog;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;

    using DataModel.BasicProperties;
    using DataModel.Summary;

    public static class MediaColorExtension
    {
        public static System.Drawing.Color ToDrawingColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class SessionSummaryExporter
    {
        private const string SummarySheet = "Summary";
        private const string LapsAndSectorsSheet = "Laps & Sectors";
        private const string PlayerLapsSheet = "Players Laps";

        public Color PersonalBestColor { get; set; } = Colors.Green;

        public Color SessionBestColor { get; set; } = Colors.Purple;

        public VelocityUnits VelocityUnits { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void ExportSessionSummary(SessionSummary sessionSummary, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileInfo newFile = new FileInfo(filePath);
                ExcelPackage package = new ExcelPackage(newFile);

                CreateWorkBook(package);
                ExcelWorkbook workbook = package.Workbook;
                AddSummary(workbook.Worksheets[SummarySheet], sessionSummary);
                AddLapsInfo(workbook.Worksheets[LapsAndSectorsSheet], sessionSummary);
                package.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to export session info");
            }

        }

        private void AddLapsInfo(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddLapsInfoHeader(sheet, sessionSummary);
            int currentColumn = 2;
            foreach (Driver driver in sessionSummary.Drivers.Where(d => d.Finished).OrderBy(o => o.FinishingPosition))
            {
                AddDriverLaps(sheet, currentColumn, driver, sessionSummary);
                currentColumn = currentColumn + 4;
            }

            foreach (Driver driver in sessionSummary.Drivers.Where(d => !d.Finished).OrderBy(d => d.TotalLaps).Reverse())
            {
                AddDriverLaps(sheet, currentColumn, driver, sessionSummary);
                currentColumn = currentColumn + 4;
            }
        }

        private void AddDriverLaps(ExcelWorksheet sheet, int startColumn, Driver driver, SessionSummary sessionSummary)
        {
            ExcelRange range = sheet.Cells[1, startColumn, 1, startColumn + 3];
            range.Merge = true;
            range.Value = driver.DriverName + "(" + (driver.Finished ? driver.FinishingPosition.ToString(): "DNF") + ")";
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[2, startColumn].Value = "Sector 1";
            sheet.Cells[2, startColumn + 1].Value = "Sector 2";
            sheet.Cells[2, startColumn + 2].Value = "Sector 3";
            sheet.Cells[2, startColumn + 3].Value = "Lap";

            int currentRow = 3;
            foreach (var lap in driver.Laps.OrderBy(l => l.LapNumber))
            {
                sheet.Cells[currentRow, startColumn].Value = FormatTimeSpan(lap.Sector1);
                if (lap == driver.BestSector1Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn]);
                }

                if (lap == sessionSummary.SessionBestSector1)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn]);
                }

                sheet.Cells[currentRow, startColumn + 1].Value = FormatTimeSpan(lap.Sector2);
                if (lap == driver.BestSector2Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 1]);
                }

                if (lap == sessionSummary.SessionBestSector2)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 1]);
                }

                sheet.Cells[currentRow, startColumn + 2].Value = FormatTimeSpan(lap.Sector3);
                if (lap == driver.BestSector3Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 2]);
                }

                if (lap == sessionSummary.SessionBestSector3)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 2]);
                }

                sheet.Cells[currentRow, startColumn + 3].Value = FormatTimeSpan(lap.LapTime);
                if (lap == driver.BestPersonalLap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 3]);
                }

                if (lap == sessionSummary.SessionBestLap)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 3]);
                }
                currentRow++;
            }

            ExcelRange outLineRange = sheet.Cells[1, startColumn, currentRow - 1, startColumn + 3];
            outLineRange.Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);
        }

        private void FormatAsPersonalBest(ExcelRange range)
        {
            FillWithColor(range, PersonalBestColor);
        }

        private void FormatAsSessionBest(ExcelRange range)
        {
            FillWithColor(range, SessionBestColor);
        }

        private void FillWithColor(ExcelRange range, Color color)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color.ToDrawingColor());
            range.Style.Fill.PatternColor.SetColor(color.ToDrawingColor());
        }

        private void AddLapsInfoHeader(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            int maxLaps = sessionSummary.Drivers.Select(d => d.TotalLaps).Max();
            sheet.SelectedRange["A1"].Value = "Lap/Driver";
            for (int i = 1; i <= maxLaps; i++)
            {
                sheet.SelectedRange["A" + (i + 2)].Value = i;
            }
            ExcelRange range = sheet.SelectedRange["A1:A" + 2 + maxLaps];
            range.Style.Font.Bold = true;
            sheet.Column(1).AutoFit();
            sheet.View.FreezePanes(1,2);
        }

        private void AddSummary(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddTrackInformation(sheet, sessionSummary);
            AddSessionBasicInformation(sheet, sessionSummary);
            AddDriversInfoHeader(sheet);
            AddDriversInfo(sheet, sessionSummary);
            WrapSummaryInTable(sheet, sessionSummary.Drivers.Count);
            AddSessionBestInfo(sessionSummary, sheet);
        }

        private static void WrapSummaryInTable(ExcelWorksheet sheet, int rowCount)
        {
            ExcelRange range = sheet.Cells[5, 1, 5 + rowCount, 9];
            ExcelTable table = sheet.Tables.Add(range, "SummaryTable");
            table.ShowHeader = true;
            for (int i = 1; i <= 9; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private void AddSessionBasicInformation(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {

            sheet.Cells["A2"].Value = "Date: " + sessionSummary.SessionRunTime.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            sheet.Cells["A3"].Value = "Time: " + sessionSummary.SessionRunTime.TimeOfDay.ToString(@"hh\:mm");
            sheet.Cells["A4"].Value = "Simulator: " + sessionSummary.Simulator;

            sheet.Cells[2,1,4,1].AutoFitColumns();

        }

        private void AddDriversInfo(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            int rowNum = 5;
            foreach (Driver driver in sessionSummary.Drivers.Where(d => d.Finished).OrderBy(driver => driver.FinishingPosition))
            {
                ExcelRow row = sheet.Row(rowNum);
                AddDriverInfo(sheet, row, driver, sessionSummary);
                rowNum++;
            }

            foreach (Driver driver in sessionSummary.Drivers.Where(d => !d.Finished).OrderBy(d => d.TotalLaps).Reverse())
            {
                ExcelRow row = sheet.Row(rowNum);
                AddDriverInfo(sheet, row, driver, sessionSummary);
                rowNum++;
            }

            for (int i = 1; i <= 9; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private void AddDriverInfo(ExcelWorksheet sheet, ExcelRow row, Driver driver, SessionSummary sessionSummary)
        {
            sheet.Cells[row.Row + 1, 1].Value = driver.Finished ? driver.FinishingPosition.ToString() : "DNF";
            sheet.Cells[row.Row + 1, 2].Value = driver.DriverName;
            sheet.Cells[row.Row + 1, 3].Value = driver.CarName;
            sheet.Cells[row.Row + 1, 4].Value = driver.TotalLaps;
            sheet.Cells[row.Row + 1, 5].Value = driver.BestPersonalLap == null ? string.Empty : FormatTimeSpan(driver.BestPersonalLap.LapTime);
            sheet.Cells[row.Row + 1, 6].Value = driver.BestSector1Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector1Lap.Sector1);
            sheet.Cells[row.Row + 1, 7].Value = driver.BestSector2Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector2Lap.Sector2);
            sheet.Cells[row.Row + 1, 8].Value = driver.BestSector3Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector3Lap.Sector3);
            sheet.Cells[row.Row + 1, 9].Value = driver.TopSpeed.GetValueInUnits(VelocityUnits).ToString("N0") + Velocity.GetUnitSymbol(VelocityUnits);

            if (driver.BestPersonalLap == sessionSummary.SessionBestLap)
            {
                sheet.Cells[row.Row + 1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 5].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
            }

            if (driver.BestSector1Lap == sessionSummary.SessionBestSector1)
            {
                sheet.Cells[row.Row + 1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 6].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
            }

            if (driver.BestSector2Lap == sessionSummary.SessionBestSector2)
            {
                sheet.Cells[row.Row + 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 7].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
            }

            if (driver.BestSector3Lap == sessionSummary.SessionBestSector3)
            {
                sheet.Cells[row.Row + 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 8].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
            }
        }

        private void AddDriversInfoHeader(ExcelWorksheet sheet)
        {
            ExcelStyle style = sheet.Cells[5,1,5,9].Style;
            style.Font.Bold = true;
            style.Font.Size = 14;
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells["A5"].Value = "#";
            sheet.Cells["B5"].Value = "Name";
            sheet.Cells["C5"].Value = "Car";
            sheet.Cells["D5"].Value = "Laps";
            sheet.Cells["E5"].Value = "Best Lap";
            sheet.Cells["F5"].Value = "Best S1";
            sheet.Cells["G5"].Value = "Best S2";
            sheet.Cells["H5"].Value = "Best S3";
            sheet.Cells["I5"].Value = "Top Speed";
        }

        private void AddTrackInformation(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            StringBuilder trackInformation = new StringBuilder(sessionSummary.SessionType.ToString());
            trackInformation.Append(" at: ");
            trackInformation.Append(sessionSummary.TrackInfo.TrackName);
            if (!string.IsNullOrWhiteSpace(sessionSummary.TrackInfo.TrackLayoutName))
            {
                trackInformation.Append(" (");
                trackInformation.Append(sessionSummary.TrackInfo.TrackLayoutName);
                trackInformation.Append(") (");
                trackInformation.Append(GetSessionLength(sessionSummary));
                trackInformation.Append(")");
            }
            sheet.Cells["A1"].Value = trackInformation.ToString();
            sheet.Cells[1,1,1,9].Merge = true;
            ExcelStyle style = sheet.Cells["A1"].Style;

            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.PatternColor.SetColor(System.Drawing.Color.Black);
            style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            style.VerticalAlignment = ExcelVerticalAlignment.Center;
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            style.Font.Color.SetColor(System.Drawing.Color.White);
            style.Font.Bold = true;
            style.Font.Size = 18;

            sheet.Row(1).Height = 35;

        }

        private void AddSessionBestInfo(SessionSummary sessionSummary, ExcelWorksheet sheet)
        {
            ExcelRange range = sheet.Cells["K5:L9"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium, SessionBestColor.ToDrawingColor());
            sheet.Cells["K5"].Value = "Session Best:";
            sheet.Cells["K6"].Value = "Sector 1:";
            sheet.Cells["K7"].Value = "Sector 2:";
            sheet.Cells["K8"].Value = "Sector 3:";
            sheet.Cells["K9"].Value = "Lap:";

            if (sessionSummary.SessionBestSector1 != null)
            {
                sheet.Cells["L6"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector1,
                    sessionSummary.SessionBestSector1.Sector1);
            }

            if (sessionSummary.SessionBestSector2 != null)
            {
                sheet.Cells["L7"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector2,
                    sessionSummary.SessionBestSector2.Sector2);
            }

            if (sessionSummary.SessionBestSector3 != null)
            {
                sheet.Cells["L8"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector3,
                    sessionSummary.SessionBestSector3.Sector3);
            }

            if (sessionSummary.SessionBestLap != null)
            {
                sheet.Cells["L9"].Value = FormatSessionBest(
                    sessionSummary.SessionBestLap,
                    sessionSummary.SessionBestLap.LapTime);
            }
            sheet.Cells["K5:K9"].Style.Font.Bold = true;
            sheet.Cells["L5:L9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            range.AutoFitColumns();

        }

        private string FormatSessionBest(Lap lap, TimeSpan timeSpan)
        {
            if (lap == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder(lap.Driver.DriverName);
            sb.Append("-Lap: ");
            sb.Append(lap.LapNumber);
            sb.Append(" | ");
            sb.Append(FormatTimeSpan(timeSpan));
            return sb.ToString();
        }

        private string GetSessionLength(SessionSummary sessionSummary)
        {
            if (sessionSummary.SessionLengthType == SessionLengthType.Laps)
            {
                return sessionSummary.TotalNumberOfLaps + " Laps";
            }
            if (sessionSummary.SessionLength.Hours > 0)
            {
                return sessionSummary.SessionLength.Hours + "h " + (sessionSummary.SessionLength.Minutes + 1) + "min" ;
            }
            return sessionSummary.SessionLength.Minutes + "mins";
        }

        private static void CreateWorkBook(ExcelPackage package)
        {
            package.Workbook.Worksheets.Add(SummarySheet);
            package.Workbook.Worksheets.Add(LapsAndSectorsSheet);
            package.Workbook.Worksheets.Add(PlayerLapsSheet);
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return "-";
            }

            // String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }

    }
}



