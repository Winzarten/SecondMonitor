namespace SecondMonitor.XslxExport.Builders
{
    using NPOI.SS.UserModel;

    public class CellStyleBuilder
    {
        private ICellStyle _cellStyle;

        private CellStyleBuilder(IWorkbook workbook)
        {
            this._cellStyle = workbook.CreateCellStyle();
        }

        public CellStyleBuilder WithFillForegroundColor(short index)
        {
            this._cellStyle.FillForegroundColor = index;
            return this;
        }

        public CellStyleBuilder WithFillPatter(FillPattern fillPattern)
        {
            this._cellStyle.FillPattern = fillPattern;
            return this;
        }

        public CellStyleBuilder WithFillBackgroundColor(short index)
        {
            this._cellStyle.FillBackgroundColor = index;
            return this;
        }

        public CellStyleBuilder WithAlignment(HorizontalAlignment alignment)
        {
            this._cellStyle.Alignment = alignment;
            return this;
        }

        public CellStyleBuilder WithVerticalAlignment(VerticalAlignment alignment)
        {
            this._cellStyle.VerticalAlignment = alignment;
            return this;
        }

        public ICellStyle Build()
        {
            return this._cellStyle;
        }
        
        public static CellStyleBuilder Create(IWorkbook workbook)
        {
            return new CellStyleBuilder(workbook);
        }
    }
}