namespace SecondMonitor.XslxExport.Builders
{
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class FontBuilder
    {
        private readonly IFont _font;

        private FontBuilder(IWorkbook workbook)
        {
            this._font = workbook.CreateFont();
        }

        public FontBuilder WithFontSize(int size)
        {
            this._font.FontHeight = size;
            return this;
        }

        public FontBuilder Bold()
        {
            this._font.IsBold = true;
            return this;
        }

        public FontBuilder WithColor(short colorIndex)
        {
            this._font.Color = colorIndex;
            return this;
        }

        public IFont Build()
        {
            return this._font;
        }

        public static FontBuilder Create(IWorkbook workbook)
        {
            return new FontBuilder(workbook);
        }
    }
}