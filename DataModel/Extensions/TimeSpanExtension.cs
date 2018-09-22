namespace SecondMonitor.DataModel.Extensions
{
    using System;

    public static class TimeSpanExtension
    {
        public const string DefaultFormat = "mm\\:ss\\.fff";

        public const string SecondOnly = "ss\\.fff";

        public static string FormatToDefault(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(DefaultFormat);
        }

        public static string FormatTimeSpanOnlySeconds(this TimeSpan timeSpan, bool includeSign = true)
        {
            if (!includeSign)
            {
                return timeSpan.ToString(SecondOnly);
            }

            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + timeSpan.ToString(SecondOnly);
            }
            else
            {
                return "+" + timeSpan.ToString(SecondOnly);
            }
        }
    }
}