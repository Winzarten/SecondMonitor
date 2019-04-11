namespace SecondMonitor.DataModel.Extensions
{
    public static class DoubleExtension
    {
        public static string ToStringScalableDecimals(this double valueD)
        {
            return valueD == 0 ? "0" : valueD < 10 ? valueD.ToString("F2") : valueD < 100 ? valueD.ToString("F1") : valueD.ToString("F0");
        }
    }
}