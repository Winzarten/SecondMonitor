namespace SecondMonitor.XslxExport
{
    using System.IO;
    using System.Text;
    using System.Windows.Media;

    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.Streaming.Values;
    using NPOI.XSSF.UserModel;

    using SecondMonitor.DataModel.Summary;

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

        private void AddSummary(ISheet sheet, SessionSummary summary)
        {
            AddTrackInformation(sheet, summary);
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
                trackInformation.Append(")");
            }
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));
            ICellStyle cellStyle = sheet.Workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Black.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.White.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont font = sheet.Workbook.CreateFont();
            font.Color = HSSFColor.White.Index;
            font.FontHeight = 18;
            font.IsBold = true;
            cellStyle.SetFont(font);
            IRow row = sheet.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(trackInformation.ToString());

            sheet.AutoSizeColumn(0);
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
