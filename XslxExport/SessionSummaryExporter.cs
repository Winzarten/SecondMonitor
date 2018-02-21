namespace SecondMonitor.XslxExport
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Media;

    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;

    using SecondMonitor.DataModel.BasicProperties;
    using SecondMonitor.DataModel.Summary;
    using SecondMonitor.XslxExport.Builders;

    public class SessionSummaryExporter
    {
        private const string SummarySheet = "Summary";
        private const string LapsAndSectorsSheet = "Laps & Sectors";
        private const string PlayerLapsSheet = "Players Laps";

        public Color PersonalBestColor { get; set; } = Colors.Green;

        public Color SessionBestColor { get; set; } = Colors.Purple;

        public void ExportSessionSummary(SessionSummary sessionSummary, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = CreateWorkBook();
                AddSummary(workbook.GetSheet(SummarySheet), sessionSummary);
                workbook.Write(fs);
            }
        }

        private void AddSummary(ISheet sheet, SessionSummary sessionSummary)
        {
            AddTrackInformation(sheet, sessionSummary);
            AddSessionBasicInformation(sheet, sessionSummary);
            AddDriversInfoHeader(sheet);
        }

        private void AddSessionBasicInformation(ISheet sheet, SessionSummary sessionSummary)
        {
            IRow row = sheet.CreateRow(1);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue("Date: " + sessionSummary.SessionRunTime.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern));

            row = sheet.CreateRow(2);
            cell = row.CreateCell(0);
            cell.SetCellValue("Time: " + sessionSummary.SessionRunTime.TimeOfDay.ToString(@"hh\:mm"));

            row = sheet.CreateRow(3);
            cell = row.CreateCell(0);
            cell.SetCellValue("Simulator: " + sessionSummary.Simulator);

            sheet.AutoSizeColumn(0);
        }

        private void AddDriversInfo(ISheet sheet, SessionSummary sessionSummary)
        {
            
        }

        private void AddDriverInfo(IRow row, Driver driver)
        {

        }

        private void AddDriversInfoHeader(ISheet sheet)
        {
            IFont font = FontBuilder.Create(sheet.Workbook).Bold().WithFontSize(14).Build();
            ICellStyle cellStyle = CellStyleBuilder.Create(sheet.Workbook).WithAlignment(HorizontalAlignment.Center)
                .Build();
            cellStyle.SetFont(font);
            IRow row = sheet.CreateRow(4);
            ICell cell = row.CreateCell(0);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("#");

            cell = row.CreateCell(0);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Name");

            cell = row.CreateCell(1);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Car");

            cell = row.CreateCell(2);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Laps");

            cell = row.CreateCell(3);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Best Lap");

            cell = row.CreateCell(4);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Best S1");

            cell = row.CreateCell(5);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Best S2");

            cell = row.CreateCell(6);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Best S3");

            cell = row.CreateCell(7);
            cell.CellStyle = cellStyle;
            cell.SetCellValue("Top Speed");

        }

        private void AddTrackInformation(ISheet sheet, SessionSummary sessionSummary)
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
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));

            ICellStyle cellStyle = CellStyleBuilder.Create(sheet.Workbook)
                .WithFillForegroundColor(HSSFColor.Black.Index).WithFillPatter(FillPattern.SolidForeground)
                .WithFillBackgroundColor(HSSFColor.White.Index).WithAlignment(HorizontalAlignment.Center)
                .WithVerticalAlignment(VerticalAlignment.Center).Build();

            IFont font = FontBuilder.Create(sheet.Workbook).WithColor(HSSFColor.White.Index).Bold().WithFontSize(18).Build();            
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(trackInformation.ToString());

            sheet.AutoSizeColumn(0);
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

        private static IWorkbook CreateWorkBook()
        {
            IWorkbook workbook = new XSSFWorkbook();
            workbook.CreateSheet(SummarySheet);
            workbook.CreateSheet(LapsAndSectorsSheet);
            workbook.CreateSheet(PlayerLapsSheet);
            return workbook;
        }

    }
}



