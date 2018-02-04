namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;

    public class TimeSpanFormatHelper
    {
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            // String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }

        public static string FormatTimeSpanOnlySeconds(TimeSpan timeSpan, bool includeSign)
        {
            string returnString = $"{(int)Math.Abs(timeSpan.TotalSeconds)}.{Math.Abs(timeSpan.Milliseconds):D3}";
            if (!includeSign)
            {
                return returnString;
            }
            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + returnString;
            }
            else
            {
                return "+" + returnString;
            }
        }
    }
}