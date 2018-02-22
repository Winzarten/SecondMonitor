namespace SecondMonitor.XslxExport
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Summary;

    public class SessionSummaryExporter
    {
        private const string SummarySheet = "Summary";
        private const string LapsAndSectorsSheet = "Laps & Sectors";
        private const string PlayerLapsSheet = "Players Laps";

        public Color PersonalBestColor { get; set; } = Colors.Green;

        public Color SessionBestColor { get; set; } = Colors.Purple;

        public VelocityUnits VelocityUnits { get; set; }

        public void ExportSessionSummary(SessionSummary sessionSummary, string filePath)
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
                package.Save();
            
        }

        private void AddSummary(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddTrackInformation(sheet, sessionSummary);
            AddSessionBasicInformation(sheet, sessionSummary);
            AddDriversInfoHeader(sheet);
            AddDriversInfo(sheet, sessionSummary);
            WrapSummaryInTable(sheet, sessionSummary.Drivers.Count);
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
            foreach (Driver driver in sessionSummary.Drivers.OrderBy(driver => driver.FinishingPosition))
            {
                ExcelRow row = sheet.Row(rowNum);
                AddDriverInfo(sheet, row, driver);
                rowNum++;
            }

            for (int i = 1; i <= 9; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private void AddDriverInfo(ExcelWorksheet sheet, ExcelRow row, Driver driver)
        {
            sheet.Cells[row.Row + 1, 1].Value = driver.FinishingPosition;
            sheet.Cells[row.Row + 1, 2].Value = driver.DriverName;
            sheet.Cells[row.Row + 1, 3].Value = driver.CarName;
            sheet.Cells[row.Row + 1, 4].Value = driver.TotalLaps;
            sheet.Cells[row.Row + 1, 9].Value = driver.TopSpeed.GetValueInUnits(VelocityUnits) + Velocity.GetUnitSymbol(VelocityUnits);            
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

        private string GetSessionLength(SessionSummary sessionSummary)
        {
            if (sessionSummary.SessionLengthType == SessionLengthType.Laps)
            {
                return sessionSummary.TotalNumberOfLaps + " Laps";
            }
            if (sessionSummary.SessionLength.Hours > 0)
            {
                return sessionSummary.SessionLength.Hours + "h " + sessionSummary.SessionLength.Minutes + "min" ;
            }
            return sessionSummary.SessionLength.Minutes + "mins";
        }

        private static void CreateWorkBook(ExcelPackage package)
        {
            package.Workbook.Worksheets.Add(SummarySheet);
            package.Workbook.Worksheets.Add(LapsAndSectorsSheet);
            package.Workbook.Worksheets.Add(PlayerLapsSheet);
        }

    }
}



