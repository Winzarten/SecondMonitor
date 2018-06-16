namespace SecondMonitor.PluginManager.Extensions
{
    using System;

    public static class StringExtensions
    {

        public static string FromArray(byte[] buffer)
        {
            if (buffer[0] == (char)0)
            {
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(buffer).Split(new[] { (char)0 }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public static string FromArray(char[] buffer)
        {
            if (buffer[0] == (char)0)
            {
                return string.Empty;
            }

            return new string(buffer);
        }
    }
}