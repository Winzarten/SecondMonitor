namespace SecondMonitor.PluginManager.Extensions
{
    using System;

    public static class StringExtensions
    {

        public static string FromArray(byte[] buffer)
        {
            return FromArray(buffer, 0);
        }

        public static string FromArray(byte[] buffer, int startIndex)
        {
            if (buffer == null || buffer[startIndex] == (char)0)
            {
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(buffer, startIndex, buffer.Length - startIndex).Split(new[] { (char)0 }, StringSplitOptions.RemoveEmptyEntries)[0];
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